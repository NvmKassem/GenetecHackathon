using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genetec_Project.Services
{
    class Problem4
    {
        static char[][] FuzzyList = new char[][] {
            new char[] { '8', 'B' },
            new char[] { 'C', 'G' },
            new char[] { 'E', 'F' },
            new char[] { 'K', 'X', 'Y' },
            new char[] { '1', 'I', 'J', 'T' },
            new char[] { '5', 'S' },
            new char[] { '0', 'D', 'O', 'Q' },
            new char[] { 'P', 'R' },
            new char[] { '2', 'Z' },
        };

        public static bool FuzzyEquals(string plate, string wanted) {
            
            for(int i = 0; i < plate.Length; i++) {
                if (plate[i].Equals(wanted[i]) || FuzzyCharacter(plate[i], wanted[i])) {
                    if (i + 1 == plate.Length)
                        return true;
                    else
                        continue;
                }
                break;
            }
            return false;
        }

        private static bool FuzzyCharacter(char char1, char char2) {
            bool lookLeft = char1 > char2;
            for (int x = 0; x < FuzzyList.Length; x++) {
                for (int y = 0; y < FuzzyList[x].Length; y++) {
                    if (FuzzyList[x][y].Equals(char1))
                        return CheckCorrespondance(x, y, char2, lookLeft);
                }
            }
            return false;
        }

        private static bool CheckCorrespondance(int x, int y, char char2, bool lookLeft) {
            if (!lookLeft)
                if (y + 1 < FuzzyList.Length)
                    for (int j = y + 1; j < FuzzyList[x].Length; j++) {
                        if (FuzzyList[x][y].Equals(char2))
                            return true;
                    }
                else
                    return false;
            else
                if (y > 0)
                    for (int j = y - 1; j >= 0; j--) {
                        if (FuzzyList[x][y].Equals(char2))
                            return true;
                    }
                else
                    return false;
            return false;
        }
    }

    
}
