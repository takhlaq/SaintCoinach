﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SaintCoinach.Graphics.Sgb {
    public class SgbFile {
        #region Struct
        [StructLayout(LayoutKind.Sequential)]
        public struct HeaderData {
            public uint Magic1;     // SGB1
            public uint FileSize;
            public uint Unknown1;
            public uint Magic2;     // SCN1

            public uint Unknown10;
            public int SharedOffset;
            public uint Unknown18;
            public int Unknown1C;

            public uint Unknown20;
            public int StatesOffset;
            public uint Unknown28;
            public uint Unknown2C;

            public uint Unknown30;
            public int SGSettingsOffset;
            public uint Unknown38;
            public uint Unknown3C;

            public uint Unknown40;
            public uint Unknown44;
            public uint Unknown48;
            public uint Unknown4C;

            public uint Unknown50;
            public uint Unknown54;
        }
        #endregion

        #region Properties
        public HeaderData Header { get; private set; }
        public SGSettings SGSettings { get; private set; }

        public Dictionary<uint, List<SGSettings.SGActionRotation>> SGActionRotationTargetMap { get; private set; }
        public Dictionary<uint, List<SGSettings.SGActionTransform2>> SGActionTransformationTargetMap { get; private set; }
        public Dictionary<uint, List<SGSettings.SGActionDoor>> SGActionDoorTargetMap { get; private set; }
        public IO.File File { get; private set; }
        public ISgbData[] Data { get; private set; }
        #endregion

        #region Constructor
        public SgbFile(IO.File file) {
            this.File = file;

            Build();
        }
        #endregion

        #region Build
        private void Build() {
            const int BaseOffset = 0x14;

            var buffer = File.GetData();
            
            this.Header = buffer.ToStructure<HeaderData>(0);
            this.SGActionRotationTargetMap = new Dictionary<uint, List<SGSettings.SGActionRotation>>();
            this.SGActionTransformationTargetMap = new Dictionary<uint, List<SGSettings.SGActionTransform2>>();
            this.SGActionDoorTargetMap = new Dictionary<uint, List<SGSettings.SGActionDoor>>();

            if (Header.Magic1 != 0x31424753 || Header.Magic2 != 0x314E4353)     // LGB1 & SCN1
                throw new System.IO.InvalidDataException();

            var data = new List<ISgbData>();

            try {
                data.Add(new SgbGroup(this, buffer, BaseOffset + Header.SharedOffset));
                this.SGSettings = new SGSettings(this, buffer, BaseOffset + Header.SGSettingsOffset);
                //data.Add(new SgbGroup(this, buffer, BaseOffset + Header.Settings, true));
                //data.Add(new SgbGroup(this, buffer, BaseOffset + Header.Unknown1));
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            this.Data = data.ToArray();
        }
        #endregion
    }
}
