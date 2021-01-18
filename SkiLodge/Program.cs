using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkiLodge
{
    class Program
    {
        static TextReader input = Console.In;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                MatrixAndParameters mountainHeightMatrix = ReadMatrixFromFile(args);
                if (mountainHeightMatrix != null)
                {
                    MatrixParametersAndPaths recordedLengthMatrix = new MatrixParametersAndPaths();
                    recordedLengthMatrix.matrix = GenerateEmptyMatrix(mountainHeightMatrix);
                    recordedLengthMatrix.height = mountainHeightMatrix.height;
                    recordedLengthMatrix.width = mountainHeightMatrix.width;
                    int y = 0;
                    int longestSlopeLength = 0;
                    coord longestSlopeCoords = new coord();
                    while (y < mountainHeightMatrix.height) 
                    {
                        int x = 0;
                        while (x < mountainHeightMatrix.width)
                        {
                            int slopeLength = FindLongestSlope(mountainHeightMatrix, recordedLengthMatrix, x, y);
                            int mountainHeight = mountainHeightMatrix.matrix[y][x];
                            if (slopeLength > longestSlopeLength)
                            {
                                longestSlopeLength = slopeLength;
                                longestSlopeCoords.x = x;
                                longestSlopeCoords.y = y;
                            }
                            else if (slopeLength == longestSlopeLength)
                            {
                                List<coord> currentPath = recordedLengthMatrix.paths[new coord() { x = x, y = y }];
                                int currentPathLowestPoint = mountainHeightMatrix.matrix[currentPath[0].y][currentPath[0].x];
                                int currentPathHighestPoint = mountainHeightMatrix.matrix[currentPath[currentPath.Count - 1].y][currentPath[currentPath.Count - 1].x];
                                int currentDrop = currentPathHighestPoint - currentPathLowestPoint;

                                List<coord> previousPath = recordedLengthMatrix.paths[new coord() { x = longestSlopeCoords.x, y = longestSlopeCoords.y }];
                                int previousPathLowestPoint = mountainHeightMatrix.matrix[previousPath[0].y][previousPath[0].x];
                                int previousPathHighestPoint = mountainHeightMatrix.matrix[previousPath[previousPath.Count - 1].y][previousPath[previousPath.Count - 1].x];
                                int previousDrop = previousPathHighestPoint - previousPathLowestPoint;

                                if ((previousDrop) < currentDrop)
                                {
                                    longestSlopeLength = slopeLength;
                                    longestSlopeCoords.x = x;
                                    longestSlopeCoords.y = y;
                                }
                            }
                            x++;
                        }
                        y++;
                    }
                    Console.WriteLine("The longest slope starts at the coordinate: ");
                    Console.WriteLine("x: " + longestSlopeCoords.x + " y: " + longestSlopeCoords.y);
                    Console.WriteLine("The slope goes through the following coordinates: ");
                    List<coord> longestPath = recordedLengthMatrix.paths[new coord() { x = longestSlopeCoords.x, y = longestSlopeCoords.y }];
                    int i = longestPath.Count -1;
                    int lowestPoint = mountainHeightMatrix.matrix[longestPath[0].y][longestPath[0].x];
                    int highestPoint = mountainHeightMatrix.matrix[longestPath[i].y][longestPath[i].x];
                    while (i >= 0)
                    {
                        Console.WriteLine("x: " + longestPath[i].x + " y: " + longestPath[i].y);
                        Console.WriteLine("height: " + mountainHeightMatrix.matrix[longestPath[i].y][longestPath[i].x]);
                        Console.WriteLine();
                        i--;
                    }
                    Console.WriteLine("HighestPoint: " + highestPoint + " LowestPoint: " + lowestPoint);
                    int drop = (highestPoint - lowestPoint);
                    Console.WriteLine("Drop: "+ drop);
                    Console.WriteLine("Longest Path: " + longestSlopeLength);
                    Console.WriteLine();
                    Console.WriteLine();
                }

            }
            else 
            {
                Console.WriteLine("Se debe pasar como argumento el nombre o localización del archivo a procesar");
            }
            
        }

        private static int[][] GenerateEmptyMatrix(MatrixAndParameters mountainHeightMatrix)
        {
            int[][] lenghtPerMemberMatrix = new int[mountainHeightMatrix.height][];
            int i = 0;
            while (i < mountainHeightMatrix.height)
            {
                lenghtPerMemberMatrix[i] = new int[mountainHeightMatrix.width];
                i++;
            }

            return lenghtPerMemberMatrix;
        }

        public class MatrixAndParameters
        {
            public int[][] matrix;
            public int width;
            public int height; 
        }
        public class MatrixParametersAndPaths: MatrixAndParameters
        {
            public Dictionary<coord,List<coord>> paths = new Dictionary<coord, List<coord>>();
        }
        public struct coord 
        {
            public int x;
            public int y;
        }
        private static MatrixAndParameters ReadMatrixFromFile(string[] args)
        {
            var filePath = Path.GetFullPath(args[0]);
            if (File.Exists(filePath))
            {
                input = File.OpenText(Path.GetFullPath(filePath));
                var readLine = input.ReadLine();
                string[] lodgeSize = readLine.Split(' ');
                int width = int.Parse(lodgeSize[0]);
                int height = int.Parse(lodgeSize[1]);
                readLine = input.ReadLine();
                int[][] mountainHeightMatrix = new int[height][];
                int y = 0;
                while (readLine != null)
                {
                    mountainHeightMatrix[y] = Array.ConvertAll(readLine.Split(' '), int.Parse);
                    readLine = input.ReadLine();
                    y++;
                }
                MatrixAndParameters result = new MatrixAndParameters();
                result.matrix = mountainHeightMatrix;
                result.height = height;
                result.width = width;
                return result;
            }
            else
            {
                Console.WriteLine("No se pudo conseguir el archivo que se pasó como argumento");
                return null;
            }
        }

        private static void WriteMatrixInConsole(MatrixAndParameters mountainHeightMatrix)
        {
            Console.WriteLine(mountainHeightMatrix.height + " " + mountainHeightMatrix.width);
            foreach (int[] arr in mountainHeightMatrix.matrix)
            {
                foreach (int mountHeight in arr)
                {
                    Console.Write(mountHeight + " ");
                }
                Console.WriteLine();
            }
        }
        private static int FindLongestSlope(MatrixAndParameters mountainHeightMatrix, MatrixParametersAndPaths recordedLengthMatrix, int xCoordinate, int yCoordinate)
        {
            int currentMemberRecordedLength = recordedLengthMatrix.matrix[yCoordinate][xCoordinate];
            int currentMemberHeight = mountainHeightMatrix.matrix[yCoordinate][xCoordinate];
            if (currentMemberRecordedLength <= 0)
            {
                //Check left
                int leftValue = 0;
                if ((xCoordinate - 1) >= 0)
                {
                    int leftMemberHeight = mountainHeightMatrix.matrix[yCoordinate][(xCoordinate - 1)];
                    if (leftMemberHeight < currentMemberHeight)
                    {
                        leftValue = FindLongestSlope(mountainHeightMatrix, recordedLengthMatrix, (xCoordinate - 1), yCoordinate);
                    }
                }
                //Check up
                int upValue = 0;
                if ((yCoordinate - 1) >= 0)
                {
                    int upMemberHeight = mountainHeightMatrix.matrix[(yCoordinate - 1)][xCoordinate];                    
                    if (upMemberHeight < mountainHeightMatrix.matrix[yCoordinate][xCoordinate])
                    {
                        upValue = FindLongestSlope(mountainHeightMatrix, recordedLengthMatrix, xCoordinate, (yCoordinate - 1));
                    }
                }
                //Check right
                int rightValue = 0;
                if ((xCoordinate + 1) <= recordedLengthMatrix.width - 1)
                {
                    int rightMemberHeight = mountainHeightMatrix.matrix[yCoordinate][(xCoordinate + 1)];
                    if (rightMemberHeight < mountainHeightMatrix.matrix[yCoordinate][xCoordinate])
                    {
                        rightValue = FindLongestSlope(mountainHeightMatrix, recordedLengthMatrix, xCoordinate + 1, yCoordinate);
                    }
                }
                //Check down
                int downValue = 0;
                if ((yCoordinate + 1) <= recordedLengthMatrix.height - 1)
                {
                    int downMemberHeight = mountainHeightMatrix.matrix[(yCoordinate + 1)][xCoordinate];
                    if (downMemberHeight < mountainHeightMatrix.matrix[yCoordinate][xCoordinate])
                    {
                        downValue = FindLongestSlope(mountainHeightMatrix, recordedLengthMatrix, xCoordinate, yCoordinate + 1);
                    }
                }
                int[] directionalValues = new int[] { leftValue, upValue, rightValue, downValue };
                int maxLength = directionalValues.Max();
                int currentMemberLength = maxLength + 1;
                recordedLengthMatrix.matrix[yCoordinate][xCoordinate] = currentMemberLength;
                if (maxLength > 0)
                {
                    if (leftValue == maxLength)
                    {
                        List<coord> currentMatrixPath = new List<coord>(recordedLengthMatrix.paths[new coord() { x = (xCoordinate - 1), y = yCoordinate }]);
                        currentMatrixPath.Add(new coord() { x = xCoordinate, y = yCoordinate });
                        recordedLengthMatrix.paths.Add(new coord() { x = xCoordinate, y = yCoordinate }, currentMatrixPath);
                    }
                    else if (upValue == maxLength)
                    {
                        List<coord> currentMatrixPath = new List<coord>(recordedLengthMatrix.paths[new coord() { x = xCoordinate, y = (yCoordinate - 1)}]);
                        currentMatrixPath.Add(new coord() { x = xCoordinate, y = yCoordinate });
                        recordedLengthMatrix.paths.Add(new coord() { x = xCoordinate, y = yCoordinate }, currentMatrixPath);
                    }
                    else if (rightValue == maxLength)
                    {
                        List<coord> currentMatrixPath = new List<coord>(recordedLengthMatrix.paths[new coord() { x = (xCoordinate + 1), y = yCoordinate}]);
                        currentMatrixPath.Add(new coord() { x = xCoordinate, y = yCoordinate });
                        recordedLengthMatrix.paths.Add(new coord() { x = xCoordinate, y = yCoordinate }, currentMatrixPath);
                    }
                    else if (downValue == maxLength)
                    {
                        List<coord> currentMatrixPath = new List<coord>(recordedLengthMatrix.paths[new coord() { x = xCoordinate, y = (yCoordinate + 1) }]);
                        currentMatrixPath.Add(new coord() { x = xCoordinate, y = yCoordinate });
                        recordedLengthMatrix.paths.Add(new coord() { x = xCoordinate, y = yCoordinate }, currentMatrixPath);
                    }
                }
                else 
                {
                    recordedLengthMatrix.paths.Add(new coord() { x = xCoordinate, y = yCoordinate}, new List<coord> { new coord() { x = xCoordinate, y = yCoordinate } });
                }
                return currentMemberLength;
            }
            else 
            {
                return currentMemberRecordedLength;
            }
        }
    }
}
