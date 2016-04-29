using System;

namespace minion.powers
{
    static class ImaginaryFriend
    {
        public static Random MagicNumberThinkerUpper = new Random();

        public static string Weapon
        {
            get
            {
                var words = new[]
                {
                    "battle axe",
                    "long sword",
                    "shuriken",
                    "mace",
                    "laser ray combobulator"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string WeaponAdjective
        {
            get
            {
                var words = new[]
                {
                    "sturdy",
                    "worn",
                    "old",
                    "tired",
                    "magical",
                    "trusty"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string WeaponPreparation
        {
            get
            {
                var words = new[]
                {
                    "fondled",
                    "sharpened",
                    "swung anticipatorily",
                    "admired",
                    "polished",
                    "cuddled"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string InconclusiveAction
        {
            get
            {
                var words = new[]
                {
                    "considered",
                    "dreamed about",
                    "flirted with the idea of",
                    "neglected to",
                    "tried",
                    "contemplated"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string ConclusiveAction
        {
            get
            {
                var words = new[]
                {
                    "decided",
                    "resolved"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string NotFarmAnimals
        {
            get
            {
                var words = new[]
                {
                    "rabbits",
                    "dragons",
                    "goldfishes",
                    "rhinocerii",
                    "fungi"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string FarmActivity
        {
            get
            {
                var words = new[]
                {
                    "herding",
                    "shepherding",
                    "grazing",
                    "cultivating",
                    "tending"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string Occupation
        {
            get
            {
                var words = new[]
                {
                    "publican",
                    "belly dancer",
                    "forager",
                    "caveman",
                    "arctic explorer"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string Place
        {
            get
            {
                var words = new[]
                {
                    "antartica",
                    "japan",
                    "fiji",
                    "venezuela",
                    "birmingham",
                    "newfoundland",
                    "auckland"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string Wearable
        {
            get
            {
                var words = new[]
                {
                    "cloak",
                    "cape",
                    "boots",
                    "trousers",
                    "hood"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string Appearance
        {
            get
            {
                var words = new[]
                {
                    "presented"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string Position
        {
            get
            {
                var words = new[]
                {
                    "my post",
                    "the town square",
                    "the fortress",
                    "the castle"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string LastRite
        {
            get
            {
                var words = new[]
                {
                    "said prayers for",
                    "hooded",
                    "blindfolded"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string CondemnedAdjective
        {
            get
            {
                var words = new[]
                {
                    "unfortunate",
                    "condemned",
                    "luckless",
                    "poor"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }

        public static string Condemned
        {
            get
            {
                var words = new[]
                {
                    "evildoer",
                    "ingrate",
                    "wretch"
                };
                return words[MagicNumberThinkerUpper.Next(0, words.Length)];
            }
        }
    }
}
