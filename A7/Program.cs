using A7.DataStructures;
using A7.Enums;
using A7.Helpers;
using A7.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace A7
{
    class Program
    {
        private static readonly System.Text.Encoding Encoding = new System.Text.UTF8Encoding();

        static async Task Main()
        {
            // Pradinio vektoriaus ilgis == generuojančios matricos eilučių skaičius
            // Užkoduoto vektoriaus ilgis N = 2^M
            // R <= M
            // Taiso jeigu kanale buvo padaryta mažiau nei 0.5(2^(m−r) − 1) klaidų.

            while (true)
            {
                try
                {
                    ParseInitialParams(out var r, out var m, out var q, out var scenario);

                    var matrix = MatrixHelper.CreateGeneratorMatrix(m, r);
                    var expectedLength = matrix.GetRowsCount();

                    switch (scenario)
                    {
                        case Scenario.BinaryVector:
                            HandleVector(r, m, q, matrix, expectedLength);
                            break;
                        case Scenario.Text:
                            HandleText(r, m, q, matrix, expectedLength);
                            break;
                        case Scenario.Picture:
                            await HandlePicture(r, m, q, matrix, expectedLength);
                            break;
                        default:
                            Console.WriteLine("Unknown scenario");
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Something went wrong. Please try again.");
                }
            }
        }

        /// <summary>
        /// Nuskaito pradinius parametrus
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        /// <param name="q"></param>
        /// <param name="scenario"></param>
        private static void ParseInitialParams(out int r, out int m, out double q, out Scenario scenario)
        {
            Console.WriteLine("Enter R: ");
            var userR = Console.ReadLine();
            while (!int.TryParse(userR, out r))
            {
                Console.WriteLine($"'{userR}' is not a valid input. Please enter valid integer.");
                userR = Console.ReadLine();
            }

            Console.WriteLine("Enter M: ");
            var userM = Console.ReadLine();
            while (!int.TryParse(userM, out m) || (m < r))
            {
                Console.WriteLine($"'{userM}' is not a valid input. Please enter valid integer that is greater than or equal to {r}.");
                userM = Console.ReadLine();
            }

            Console.WriteLine($"Code fixes no more than '{0.5 * (Math.Pow(2, m - r) - 1)}' mistakes.");

            Console.WriteLine("Enter Q (simulated channel mistake probability): ");
            var userQ = Console.ReadLine().Replace(',', '.');
            while (!double.TryParse(userQ, out q) || (q < 0 || q > 1))
            {
                Console.WriteLine($"'{userQ}' is not a valid input. Please enter double between 0 and 1");
                userQ = Console.ReadLine().Replace(',', '.');
            }

            Console.WriteLine("Enter scenario. 0 - vector, 1 - text, 2 - picture");
            var userScenario = Console.ReadLine();
            while (!Enum.TryParse(userScenario, out scenario) || !Enum.IsDefined(typeof(Scenario), scenario))
            {
                Console.WriteLine($"'{userScenario}' is not a valid input. Please enter number between 0 and 2.");
                userScenario = Console.ReadLine();
            }
        }

        /// <summary>
        /// Paveikslėlio apdorojimas
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        /// <param name="q"></param>
        /// <param name="matrix"></param>
        /// <param name="expectedLength"></param>
        /// <returns></returns>
        private static async Task HandlePicture(int r, int m, double q, Matrix matrix, int expectedLength)
        {
            Console.WriteLine("Select available photos. 0 - bird, 1 - fire, 2 - robot");
            var selectedPhoto = Console.ReadLine();
            AvailablePhoto photo;
            while (!Enum.TryParse(selectedPhoto, out photo) || !Enum.IsDefined(typeof(AvailablePhoto), photo))
            {
                Console.WriteLine($"'{selectedPhoto}' is not a valid input. Please enter number between 0 and 2.");
                selectedPhoto = Console.ReadLine();
            }

            var basePath = Path.Combine(Environment.CurrentDirectory, "Resources");
            var filepath = Path.Combine(basePath, $"{photo}.bmp");
            if (!File.Exists(filepath))
            {
                Console.WriteLine("File does not exist.");
                return;
            }

            // Paveikslėlio skaitymas ir konvertavimas į baitų masyvą.
            var (pictureHeader, pictureBody) = await PictureHelper.ReadBmpImageAsync(filepath);
            var (initialVectors, addedBits) = ConversionService.ConvertByteArrayToVectors(pictureBody, expectedLength);

            // Vektorių kodavimas
            var encodedVectors = new List<int[]>();
            initialVectors.ForEach(x => encodedVectors.Add(Encoder.EncodeVector(x, matrix)));

            // Siuntimas kanalu
            var distortedVectors = new List<int[]>();
            var distortedPlainVectors = new List<int[]>();
            encodedVectors.SkipLast(1).ToList().ForEach(x => distortedVectors.Add(Channel.Send(x, q)));
            distortedVectors.Add(Channel.Send(encodedVectors.Last(), q, addedBits));
            initialVectors.ForEach(x => distortedPlainVectors.Add(Channel.Send(x, q)));

            // Vektorių dekodavimas
            var decodedVectors = new List<int[]>();
            distortedVectors.ForEach(x => decodedVectors.Add(Decoder.DecodeVector(x, matrix, m, r)));

            var decodedByteArr = ConversionService.ConvertVectorsToByteArray(decodedVectors, addedBits);
            var plainByteArr = ConversionService.ConvertVectorsToByteArray(distortedPlainVectors, addedBits);

            // Paveikslėlių išsaugojimas.
            var task1 = PictureHelper.WriteBmpImageAsync(basePath, pictureHeader, decodedByteArr);
            var task2 = PictureHelper.WriteBmpImageAsync(basePath, pictureHeader, plainByteArr, false);
            Task.WaitAll(task1, task2);

            filepath = Path.GetFullPath(basePath);
            System.Diagnostics.Process.Start("explorer.exe", filepath);
        }

        /// <summary>
        /// Teksto apdorojimas
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        /// <param name="q"></param>
        /// <param name="matrix"></param>
        /// <param name="expectedLength"></param>
        private static void HandleText(int r, int m, double q, Matrix matrix, int expectedLength)
        {
            Console.WriteLine("Please enter your text");
            var text = Console.ReadLine();
            Console.WriteLine("Initial text: ".PadRight(20) + text);

            // Teksto konvertavimas į baitų masyvą.
            var byteArray = Encoding.GetBytes(text);
            var (initialVectors, addedBits) = ConversionService.ConvertByteArrayToVectors(byteArray, expectedLength);

            // Vektorių kodavimas
            var encodedVectors = new List<int[]>();
            initialVectors.ForEach(x => encodedVectors.Add(Encoder.EncodeVector(x, matrix)));

            // Siuntimas kanalu
            var distortedVectors = new List<int[]>();
            var distortedPlainVectors = new List<int[]>();
            encodedVectors.SkipLast(1).ToList().ForEach(x => distortedVectors.Add(Channel.Send(x, q)));
            distortedVectors.Add(Channel.Send(encodedVectors.Last(), q, addedBits));
            initialVectors.ForEach(x => distortedPlainVectors.Add(Channel.Send(x, q)));

            // Vektorių dekodavimas
            var decodedVectors = new List<int[]>();
            distortedVectors.ForEach(x => decodedVectors.Add(Decoder.DecodeVector(x, matrix, m, r)));

            var decodedText = Encoding.GetString(ConversionService.ConvertVectorsToByteArray(decodedVectors, addedBits));
            var decodedTextWithoutEncoding = Encoding.GetString(ConversionService.ConvertVectorsToByteArray(distortedPlainVectors, addedBits));
            
            // Replacing carriage return and new line char.
            decodedText= decodedText.Replace('\r', 'r');
            decodedText= decodedText.Replace('\n', 'n');
            decodedTextWithoutEncoding = decodedTextWithoutEncoding.Replace('\r', 'r');
            decodedTextWithoutEncoding = decodedTextWithoutEncoding.Replace('\n', 'n');

            Console.WriteLine("Without encoding: ".PadRight(20) + decodedTextWithoutEncoding);
            Console.WriteLine("Decoded text: ".PadRight(20) + decodedText);
        }

        /// <summary>
        /// Vektoriaus apdorojimas
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        /// <param name="q"></param>
        /// <param name="matrix"></param>
        /// <param name="expectedLength"></param>
        private static void HandleVector(int r, int m, double q, Matrix matrix, int expectedLength)
        {
            Console.WriteLine($"Please enter binary vector of length {expectedLength}");
            string binaryString;
            do
            {
                binaryString = Console.ReadLine();
                if (binaryString.Length == expectedLength) continue;
                Console.WriteLine($"Input is not equal to expected length of {expectedLength}");
            } while (binaryString.Length != expectedLength);

            // Binarinė simbolių eilutė konvertuojama į vektorių.
            var initialVector = ConversionService.ConvertBinaryStringToVector(binaryString);
            Console.WriteLine("Input vector: ".PadRight(20) + string.Join(",", initialVector));
            
            // Vektorio kodavimas
            var encodedVector = Encoder.EncodeVector(initialVector, matrix);
            Console.WriteLine("Encoded vector: ".PadRight(20) + string.Join(",", encodedVector));

            // Siuntimas kanalu
            var distortedVector = Channel.Send(encodedVector, q);

            // Klaidingų pozicijų gavimas
            var errornousPositions = Channel.GetErrornousPositions(encodedVector, distortedVector);
            Console.WriteLine("Distorted vector: ".PadRight(20) + string.Join(",", distortedVector));
            var errornousPositionsString = Enumerable.Repeat(" ", distortedVector.Length).ToArray();
            for (var i = 0; i < errornousPositions.Count; i++)
            {
                errornousPositionsString[errornousPositions[i]] = "^";
            }
            Console.WriteLine(" ".PadRight(20) + string.Join(" ", errornousPositionsString));
            Console.WriteLine("Error positions: ".PadRight(20) + string.Join(",", errornousPositions));
            Console.WriteLine("Errors count: ".PadRight(20) + string.Join(",", errornousPositions.Count));

            // Pasirinktų pozicijų pakeitimas
            Console.WriteLine("Do you want to modify distorted vector? Press 'Y' for YES, any key - NO");
            if (Console.ReadLine().Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Select positions you want to reverse. Example: 2, 4, 8");
                var positionsToReverse = new List<int>();
                var userPositionInput = Console.ReadLine().Split(",");
                foreach (var pos in userPositionInput)
                {
                    if (int.TryParse(pos, out var vectorIndex) && vectorIndex < distortedVector.Length)
                    {
                        positionsToReverse.Add(vectorIndex);
                    }
                }

                distortedVector = Channel.FixDistortedVector(distortedVector, positionsToReverse);
                Console.WriteLine("Encoded vector: ".PadRight(20) + string.Join(",", encodedVector));
                Console.WriteLine("Distorted vector: ".PadRight(20) + string.Join(",", distortedVector));

                errornousPositions = Channel.GetErrornousPositions(encodedVector, distortedVector);
                Console.WriteLine("Errors count: ".PadRight(20) + string.Join(",", errornousPositions.Count));
            }

            // Vektorio dekodavimas
            var decodedVector = Decoder.DecodeVector(distortedVector, matrix, m, r);
            Console.WriteLine("Decoded vector: ".PadRight(20) + string.Join(",", decodedVector));

            errornousPositions = Channel.GetErrornousPositions(initialVector, decodedVector);
            if (errornousPositions.Count == 0)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("Decoded succesfully!");
                Console.WriteLine("--------------------");
            }
            else
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("Decoded with errors.");
                Console.WriteLine("--------------------");
            }
        }
    }
}
