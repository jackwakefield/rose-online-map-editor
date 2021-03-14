using System.Collections.Generic;
using System.Text;

namespace Map_Editor.Engine.Character
{
    /// <summary>
    /// CHR class.
    /// </summary>
    public class CHR
    {
        /// <summary>
        /// Character class.
        /// </summary>
        public class Character
        {
            #region Sub Classes

            /// <summary>
            /// Motion class.
            /// </summary>
            public class Motion
            {
                #region Member Declarations

                /// <summary>
                /// Gets or sets the ID.
                /// </summary>
                /// <value>The ID.</value>
                public short ID { get; set; }

                /// <summary>
                /// Gets or sets the motion ID.
                /// </summary>
                /// <value>The motion ID.</value>
                public short MotionID { get; set; }

                #endregion
            }

            /// <summary>
            /// Effect class.
            /// </summary>
            public class Effect
            {
                #region Member Declarations

                /// <summary>
                /// Gets or sets the ID.
                /// </summary>
                /// <value>The ID.</value>
                public short ID { get; set; }

                /// <summary>
                /// Gets or sets the effect ID.
                /// </summary>
                /// <value>The effect ID.</value>
                public short EffectID { get; set; }

                #endregion
            }

            #endregion

            #region Member Declarations

            /// <summary>
            /// Gets or sets a value indicating whether this instance is active.
            /// </summary>
            /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
            public bool IsActive { get; set; }

            /// <summary>
            /// Gets or sets the bone ID.
            /// </summary>
            /// <value>The bone ID.</value>
            public short BoneID { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            #endregion

            #region List Declarations

            /// <summary>
            /// Gets or sets the models.
            /// </summary>
            /// <value>The models.</value>
            public List<short> Models { get; set; }

            /// <summary>
            /// Gets or sets the motions.
            /// </summary>
            /// <value>The motions.</value>
            public List<Motion> Motions { get; set; }

            /// <summary>
            /// Gets or sets the effects.
            /// </summary>
            /// <value>The effects.</value>
            public List<Effect> Effects { get; set; }

            #endregion
        }

        #region Member Declarations

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; private set; }

        #endregion

        #region List Declarations

        /// <summary>
        /// Gets or sets the bones.
        /// </summary>
        /// <value>The bones.</value>
        public List<string> Bones { get; set; }

        /// <summary>
        /// Gets or sets the motions.
        /// </summary>
        /// <value>The motions.</value>
        public List<string> Motions { get; set; }

        /// <summary>
        /// Gets or sets the effects.
        /// </summary>
        /// <value>The effects.</value>
        public List<string> Effects { get; set; }

        /// <summary>
        /// Gets or sets the characters.
        /// </summary>
        /// <value>The characters.</value>
        public List<Character> Characters { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CHR"/> class.
        /// </summary>
        public CHR()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CHR"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public CHR(string filePath)
        {
            Load(filePath);
        }

        /// <summary>
        /// Loads the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Load(string filePath)
        {
            FileHandler fh = new FileHandler(FilePath = filePath, FileHandler.FileOpenMode.Reading, Encoding.GetEncoding("EUC-KR"));

            short boneCount = fh.Read<short>();
            Bones = new List<string>(boneCount);

            for (int i = 0; i < boneCount; i++)
                Bones.Add(fh.Read<ZString>());

            short motionCount = fh.Read<short>();
            Motions = new List<string>(motionCount);

            for (int i = 0; i < motionCount; i++)
                Motions.Add(fh.Read<ZString>());

            short effectCount = fh.Read<short>();
            Effects = new List<string>(effectCount);

            for (int i = 0; i < effectCount; i++)
                Effects.Add(fh.Read<ZString>());

            short characterCount = fh.Read<short>();
            Characters = new List<Character>(characterCount);

            for (int i = 0; i < characterCount; i++)
            {
                Characters.Add(new Character()
                {
                    Models = new List<short>(),
                    Motions = new List<Character.Motion>(0),
                    Effects = new List<Character.Effect>(0),
                    IsActive = fh.Read<byte>() > 0
                });

                if (Characters[i].IsActive)
                {
                    Characters[i].BoneID = fh.Read<short>();
                    Characters[i].Name = fh.Read<ZString>();

                    int modelCount = fh.Read<short>();
                    Characters[i].Models = new List<short>(modelCount);

                    for (int j = 0; j < modelCount; j++)
                        Characters[i].Models.Add(fh.Read<short>());

                    motionCount = fh.Read<short>();
                    Characters[i].Motions = new List<Character.Motion>(motionCount);

                    for (int j = 0; j < motionCount; j++)
                    {
                        Characters[i].Motions.Add(new Character.Motion()
                        {
                            ID = fh.Read<short>(),
                            MotionID = fh.Read<short>()
                        });
                    }

                    effectCount = fh.Read<short>();
                    Characters[i].Effects = new List<Character.Effect>(effectCount);

                    for (int j = 0; j < effectCount; j++)
                    {
                        Characters[i].Effects.Add(new Character.Effect()
                        {
                            ID = fh.Read<short>(),
                            EffectID = fh.Read<short>()
                        });
                    }
                }
            }

            fh.Close();
        }
    }
}