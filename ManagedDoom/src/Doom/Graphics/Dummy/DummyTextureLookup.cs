using System;
using System.Collections;
using System.Collections.Generic;

namespace ManagedDoom
{
    public class DummyTextureLookup : ITextureLookup
    {
        private readonly DummyData dummyData = new();

        private List<Texture> textures;
        private Dictionary<string, Texture> nameToTexture;
        private Dictionary<string, int> nameToNumber;

        private int[] switchList;

        public DummyTextureLookup(Wad wad)
        {
            InitLookup(wad);
            InitSwitchList();
        }

        private void InitLookup(Wad wad)
        {
            textures = [];
            nameToTexture = [];
            nameToNumber = [];

            for (int n = 1; n <= 2; n++)
            {
                int lumpNumber = wad.GetLumpNumber("TEXTURE" + n);
                if (lumpNumber == -1)
                {
                    break;
                }

                byte[] data = wad.ReadLump(lumpNumber);
                int count = BitConverter.ToInt32(data, 0);
                for (int i = 0; i < count; i++)
                {
                    int offset = BitConverter.ToInt32(data, 4 + 4 * i);
                    string name = Texture.GetName(data, offset);
                    int height = Texture.GetHeight(data, offset);
                    Texture texture = dummyData.GetTexture(height);
                    nameToNumber.TryAdd(name, textures.Count);
                    textures.Add(texture);
                    nameToTexture.TryAdd(name, texture);
                }
            }
        }

        private void InitSwitchList()
        {
            var list = new List<int>();
            foreach (Tuple<DoomString, DoomString> tuple in DoomInfo.SwitchNames)
            {
                int texNum1 = GetNumber(tuple.Item1);
                int texNum2 = GetNumber(tuple.Item2);
                if (texNum1 != -1 && texNum2 != -1)
                {
                    list.Add(texNum1);
                    list.Add(texNum2);
                }
            }
            switchList = list.ToArray();
        }

        public int GetNumber(string name)
        {
            if (name[0] == '-')
            {
                return 0;
            }

            if (nameToNumber.TryGetValue(name, out int number))
            {
                return number;
            }
            else
            {
                return -1;
            }
        }

        public IEnumerator<Texture> GetEnumerator()
        {
            return textures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return textures.GetEnumerator();
        }

        public void Initialize(Wad wad)
        {
            throw new NotImplementedException();
        }

        public int Count => textures.Count;
        public Texture this[int num] => textures[num];
        public Texture this[string name] => nameToTexture[name];
        public int[] SwitchList => switchList;
    }
}
