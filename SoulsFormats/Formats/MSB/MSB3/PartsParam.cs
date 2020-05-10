﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB3
    {
        /// <summary>
        /// Instances of various "things" in this MSB.
        /// </summary>
        public class PartsParam : Param<Part>, IMsbParam<IMsbPart>
        {
            internal override int Version => 3;
            internal override string Type => "PARTS_PARAM_ST";

            /// <summary>
            /// Map pieces in the MSB.
            /// </summary>
            public List<Part.MapPiece> MapPieces { get; set; }

            /// <summary>
            /// Objects in the MSB.
            /// </summary>
            public List<Part.Object> Objects { get; set; }

            /// <summary>
            /// Enemies in the MSB.
            /// </summary>
            public List<Part.Enemy> Enemies { get; set; }

            /// <summary>
            /// Players in the MSB.
            /// </summary>
            public List<Part.Player> Players { get; set; }

            /// <summary>
            /// Collisions in the MSB.
            /// </summary>
            public List<Part.Collision> Collisions { get; set; }

            /// <summary>
            /// Dummy objects in the MSB.
            /// </summary>
            public List<Part.DummyObject> DummyObjects { get; set; }

            /// <summary>
            /// Dummy enemies in the MSB.
            /// </summary>
            public List<Part.DummyEnemy> DummyEnemies { get; set; }

            /// <summary>
            /// Connect collisions in the MSB.
            /// </summary>
            public List<Part.ConnectCollision> ConnectCollisions { get; set; }

            /// <summary>
            /// Creates a new PartsParam with no parts.
            /// </summary>
            public PartsParam()
            {
                MapPieces = new List<Part.MapPiece>();
                Objects = new List<Part.Object>();
                Enemies = new List<Part.Enemy>();
                Players = new List<Part.Player>();
                Collisions = new List<Part.Collision>();
                DummyObjects = new List<Part.DummyObject>();
                DummyEnemies = new List<Part.DummyEnemy>();
                ConnectCollisions = new List<Part.ConnectCollision>();
            }

            /// <summary>
            /// Returns every part in the order they'll be written.
            /// </summary>
            public override List<Part> GetEntries()
            {
                return SFUtil.ConcatAll<Part>(
                    MapPieces, Objects, Enemies, Players, Collisions,
                    DummyObjects, DummyEnemies, ConnectCollisions);
            }
            IReadOnlyList<IMsbPart> IMsbParam<IMsbPart>.GetEntries() => GetEntries();

            internal override Part ReadEntry(BinaryReaderEx br)
            {
                PartsType type = br.GetEnum32<PartsType>(br.Position + 8);
                switch (type)
                {
                    case PartsType.MapPiece:
                        return MapPieces.EchoAdd(new Part.MapPiece(br));

                    case PartsType.Object:
                        return Objects.EchoAdd(new Part.Object(br));

                    case PartsType.Enemy:
                        return Enemies.EchoAdd(new Part.Enemy(br));

                    case PartsType.Player:
                        return Players.EchoAdd(new Part.Player(br));

                    case PartsType.Collision:
                        return Collisions.EchoAdd(new Part.Collision(br));

                    case PartsType.DummyObject:
                        return DummyObjects.EchoAdd(new Part.DummyObject(br));

                    case PartsType.DummyEnemy:
                        return DummyEnemies.EchoAdd(new Part.DummyEnemy(br));

                    case PartsType.ConnectCollision:
                        return ConnectCollisions.EchoAdd(new Part.ConnectCollision(br));

                    default:
                        throw new NotImplementedException($"Unsupported part type: {type}");
                }
            }
        }

        internal enum PartsType : uint
        {
            MapPiece = 0,
            Object = 1,
            Enemy = 2,
            Item = 3,
            Player = 4,
            Collision = 5,
            NPCWander = 6,
            Protoboss = 7,
            Navmesh = 8,
            DummyObject = 9,
            DummyEnemy = 10,
            ConnectCollision = 11,
        }

        /// <summary>
        /// Any instance of some "thing" in a map.
        /// </summary>
        public abstract class Part : NamedEntry, IMsbPart
        {
            internal abstract PartsType Type { get; }

            internal abstract bool HasGparamConfig { get; }
            internal abstract bool HasUnk4 { get; }

            /// <summary>
            /// The name of this part.
            /// </summary>
            public override string Name { get; set; }

            /// <summary>
            /// Unknown network path to a .sib file.
            /// </summary>
            public string SibPath { get; set; }

            /// <summary>
            /// The name of this part's model.
            /// </summary>
            public string ModelName { get; set; }
            private int ModelIndex;

            /// <summary>
            /// The center of the part.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// The rotation of the part.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// The scale of the part, which only really works right for map pieces.
            /// </summary>
            public Vector3 Scale { get; set; }

            /// <summary>
            /// A bitmask that determines which ceremonies the part appears in.
            /// </summary>
            public uint MapStudioLayer { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public uint[] DrawGroups { get; private set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public uint[] DispGroups { get; private set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public uint[] BackreadGroups { get; private set; }

            /// <summary>
            /// Used to identify the part in event scripts.
            /// </summary>
            public int EntityID { get; set; }

            /// <summary>
            /// Used to identify multiple parts with the same ID in event scripts.
            /// </summary>
            public int[] EntityGroups { get; private set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public sbyte UnkE04 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public sbyte UnkE05 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public sbyte LanternID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public sbyte LodParamID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public sbyte UnkE0E { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool PointLightShadowSource { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool ShadowSource { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool ShadowDest { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool IsShadowOnly { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool DrawByReflectCam { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool DrawOnlyReflectCam { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool UseDepthBiasFloat { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool DisablePointLightEffect { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int UnkE18 { get; set; }

            internal Part(string name)
            {
                Name = name;
                Scale = Vector3.One;
                DrawGroups = new uint[8] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                DispGroups = new uint[8] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                BackreadGroups = new uint[8] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                EntityID = -1;
                EntityGroups = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
            }

            internal Part(Part clone)
            {
                Name = clone.Name;
                SibPath = clone.SibPath;
                ModelName = clone.ModelName;
                Position = clone.Position;
                Rotation = clone.Rotation;
                Scale = clone.Scale;
                MapStudioLayer = clone.MapStudioLayer;
                DrawGroups = (uint[])clone.DrawGroups.Clone();
                DispGroups = (uint[])clone.DispGroups.Clone();
                BackreadGroups = (uint[])clone.BackreadGroups.Clone();
                EntityID = clone.EntityID;
                UnkE04 = clone.UnkE04;
                UnkE05 = clone.UnkE05;
                LanternID = clone.LanternID;
                LodParamID = clone.LodParamID;
                UnkE0E = clone.UnkE0E;
                PointLightShadowSource = clone.PointLightShadowSource;
                ShadowSource = clone.ShadowSource;
                ShadowDest = clone.ShadowDest;
                IsShadowOnly = clone.IsShadowOnly;
                DrawByReflectCam = clone.DrawByReflectCam;
                DrawOnlyReflectCam = clone.DrawOnlyReflectCam;
                UseDepthBiasFloat = clone.UseDepthBiasFloat;
                DisablePointLightEffect = clone.DisablePointLightEffect;
                UnkE18 = clone.UnkE18;
                EntityGroups = (int[])clone.EntityGroups.Clone();
            }

            internal Part(BinaryReaderEx br)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                ModelIndex = br.ReadInt32();
                br.AssertInt32(0);
                long sibOffset = br.ReadInt64();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Scale = br.ReadVector3();

                br.AssertInt32(-1);
                MapStudioLayer = br.ReadUInt32();
                DrawGroups = br.ReadUInt32s(8);
                DispGroups = br.ReadUInt32s(8);
                BackreadGroups = br.ReadUInt32s(8);
                br.AssertInt32(0);

                long baseDataOffset = br.ReadInt64();
                long typeDataOffset = br.ReadInt64();
                long gparamOffset = br.ReadInt64();
                long unkOffset4 = br.ReadInt64();

                Name = br.GetUTF16(start + nameOffset);
                SibPath = br.GetUTF16(start + sibOffset);

                br.Position = start + baseDataOffset;
                EntityID = br.ReadInt32();
                UnkE04 = br.ReadSByte();
                UnkE05 = br.ReadSByte();
                br.AssertInt16(0);
                br.AssertInt32(0);
                LanternID = br.ReadSByte();
                LodParamID = br.ReadSByte();
                UnkE0E = br.ReadSByte();
                PointLightShadowSource = br.ReadBoolean();
                ShadowSource = br.ReadBoolean();
                ShadowDest = br.ReadBoolean();
                IsShadowOnly = br.ReadBoolean();
                DrawByReflectCam = br.ReadBoolean();
                DrawOnlyReflectCam = br.ReadBoolean();
                UseDepthBiasFloat = br.ReadBoolean();
                DisablePointLightEffect = br.ReadBoolean();
                br.AssertByte(0);
                UnkE18 = br.ReadInt32();
                EntityGroups = br.ReadInt32s(8);
                br.AssertInt32(0);

                br.Position = start + typeDataOffset;
                ReadTypeData(br);

                if (HasGparamConfig)
                {
                    br.Position = start + gparamOffset;
                    ReadGparamConfig(br);
                }

                if (HasUnk4)
                {
                    br.Position = start + unkOffset4;
                    ReadUnk4(br);
                }
            }

            internal abstract void ReadTypeData(BinaryReaderEx br);

            internal virtual void ReadGparamConfig(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Gparam config should not be read for parts with no gparam config.");
            }

            internal virtual void ReadUnk4(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Unk struct 4 should not be read for parts with no unk struct 4.");
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.WriteInt32(ModelIndex);
                bw.WriteInt32(0);
                bw.ReserveInt64("SibOffset");
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteVector3(Scale);

                bw.WriteInt32(-1);
                bw.WriteUInt32(MapStudioLayer);
                bw.WriteUInt32s(DrawGroups);
                bw.WriteUInt32s(DispGroups);
                bw.WriteUInt32s(BackreadGroups);
                bw.WriteInt32(0);

                bw.ReserveInt64("BaseDataOffset");
                bw.ReserveInt64("TypeDataOffset");
                bw.ReserveInt64("GparamOffset");
                bw.ReserveInt64("UnkOffset4");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(MSB.ReambiguateName(Name), true);
                bw.FillInt64("SibOffset", bw.Position - start);
                bw.WriteUTF16(SibPath, true);
                // This is purely here for byte-perfect writes because From is nasty
                if (SibPath == "")
                    bw.WritePattern(0x24, 0x00);
                bw.Pad(8);

                bw.FillInt64("BaseDataOffset", bw.Position - start);
                bw.WriteInt32(EntityID);

                bw.WriteSByte(UnkE04);
                bw.WriteSByte(UnkE05);
                bw.WriteInt16(0);

                bw.WriteInt32(0);

                bw.WriteSByte(LanternID);
                bw.WriteSByte(LodParamID);
                bw.WriteSByte(UnkE0E);
                bw.WriteBoolean(PointLightShadowSource);

                bw.WriteBoolean(ShadowSource);
                bw.WriteBoolean(ShadowDest);
                bw.WriteBoolean(IsShadowOnly);
                bw.WriteBoolean(DrawByReflectCam);

                bw.WriteBoolean(DrawOnlyReflectCam);
                bw.WriteBoolean(UseDepthBiasFloat);
                bw.WriteBoolean(DisablePointLightEffect);
                bw.WriteByte(0);

                bw.WriteInt32(UnkE18);
                bw.WriteInt32s(EntityGroups);
                bw.WriteInt32(0);
                bw.Pad(8);

                bw.FillInt64("TypeDataOffset", bw.Position - start);
                WriteTypeData(bw);

                if (HasGparamConfig)
                {
                    bw.FillInt64("GparamOffset", bw.Position - start);
                    WriteGparamConfig(bw);
                }
                else
                {
                    bw.FillInt64("GparamOffset", 0);
                }

                if (HasUnk4)
                {
                    bw.FillInt64("UnkOffset4", bw.Position - start);
                    WriteUnk4(bw);
                }
                else
                {
                    bw.FillInt64("UnkOffset4", 0);
                }
            }

            internal abstract void WriteTypeData(BinaryWriterEx bw);

            internal virtual void WriteGparamConfig(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Gparam config should not be written for parts with no gparam config.");
            }

            internal virtual void WriteUnk4(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Unk struct 4 should not be written for parts with no unk struct 4.");
            }

            internal virtual void GetNames(MSB3 msb, Entries entries)
            {
                ModelName = MSB.FindName(entries.Models, ModelIndex);
            }

            internal virtual void GetIndices(MSB3 msb, Entries entries)
            {
                ModelIndex = MSB.FindIndex(entries.Models, ModelName);
            }

            /// <summary>
            /// Returns the type and name of this part.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} : {Name}";
            }

            /// <summary>
            /// Gparam value IDs for various part types.
            /// </summary>
            public class GparamConfig
            {
                /// <summary>
                /// ID of the value set from LightSet ParamEditor to use.
                /// </summary>
                public int LightSetID { get; set; }

                /// <summary>
                /// ID of the value set from FogParamEditor to use.
                /// </summary>
                public int FogParamID { get; set; }

                /// <summary>
                /// ID of the value set from LightScattering : ParamEditor to use.
                /// </summary>
                public int LightScatteringID { get; set; }

                /// <summary>
                /// ID of the value set from Env Map:Editor to use.
                /// </summary>
                public int EnvMapID { get; set; }

                /// <summary>
                /// Creates a GparamConfig with default values.
                /// </summary>
                public GparamConfig() { }

                /// <summary>
                /// Clones an existing GparamConfig.
                /// </summary>
                public GparamConfig(GparamConfig clone)
                {
                    LightSetID = clone.LightSetID;
                    FogParamID = clone.FogParamID;
                    LightScatteringID = clone.LightScatteringID;
                    EnvMapID = clone.EnvMapID;
                }

                internal GparamConfig(BinaryReaderEx br)
                {
                    LightSetID = br.ReadInt32();
                    FogParamID = br.ReadInt32();
                    LightScatteringID = br.ReadInt32();
                    EnvMapID = br.ReadInt32();
                    br.AssertPattern(0x10, 0x00);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(LightSetID);
                    bw.WriteInt32(FogParamID);
                    bw.WriteInt32(LightScatteringID);
                    bw.WriteInt32(EnvMapID);
                    bw.WritePattern(0x10, 0x00);
                }

                /// <summary>
                /// Returns the four gparam values as a string.
                /// </summary>
                public override string ToString()
                {
                    return $"{LightSetID}, {FogParamID}, {LightScatteringID}, {EnvMapID}";
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class UnkStruct4
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk3C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float Unk40 { get; set; }

                /// <summary>
                /// Creates an UnkStruct4 with default values.
                /// </summary>
                public UnkStruct4() { }

                /// <summary>
                /// Clones an existing UnkStruct4.
                /// </summary>
                public UnkStruct4(UnkStruct4 clone)
                {
                    Unk3C = clone.Unk3C;
                    Unk40 = clone.Unk40;
                }

                internal UnkStruct4(BinaryReaderEx br)
                {
                    br.AssertPattern(0x3C, 0x00);
                    Unk3C = br.ReadInt32();
                    Unk40 = br.ReadSingle();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WritePattern(0x3C, 0x00);
                    bw.WriteInt32(Unk3C);
                    bw.WriteSingle(Unk40);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A static model making up the map.
            /// </summary>
            public class MapPiece : Part
            {
                internal override PartsType Type => PartsType.MapPiece;

                internal override bool HasGparamConfig => true;
                internal override bool HasUnk4 => false;

                /// <summary>
                /// Gparam IDs for this map piece.
                /// </summary>
                public GparamConfig Gparam { get; set; }

                /// <summary>
                /// Creates a new MapPiece with the given name.
                /// </summary>
                public MapPiece(string name) : base(name)
                {
                    Gparam = new GparamConfig();
                }

                /// <summary>
                /// Creates a new MapPiece with values copied from another.
                /// </summary>
                public MapPiece(MapPiece clone) : base(clone)
                {
                    Gparam = new GparamConfig(clone.Gparam);
                }

                internal MapPiece(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);
            }

            /// <summary>
            /// Any dynamic object such as elevators, crates, ladders, etc.
            /// </summary>
            public class Object : Part
            {
                internal override PartsType Type => PartsType.Object;

                internal override bool HasGparamConfig => true;
                internal override bool HasUnk4 => false;

                /// <summary>
                /// Gparam IDs for this object.
                /// </summary>
                public GparamConfig Gparam { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public string CollisionName { get; set; }
                private int CollisionPartIndex;

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool EnableObjAnimNetSyncStructure { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool CollisionFilter { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool SetMainObjStructureBooleans { get; set; }

                /// <summary>
                /// Automatically playing animations; only the first is actually used, according to Pav.
                /// </summary>
                public short[] AnimIDs { get; private set; }

                /// <summary>
                /// Value added to the base ModelSfxParam ID; only the first is actually used, according to Pav.
                /// </summary>
                public short[] ModelSfxParamRelativeIDs { get; private set; }

                /// <summary>
                /// Creates a new Object with the given name.
                /// </summary>
                public Object(string name) : base(name)
                {
                    Gparam = new GparamConfig();
                    AnimIDs = new short[4] { -1, -1, -1, -1 };
                    ModelSfxParamRelativeIDs = new short[4] { -1, -1, -1, -1 };
                }

                /// <summary>
                /// Creates a new Object with values copied from another.
                /// </summary>
                public Object(Object clone) : base(clone)
                {
                    Gparam = new GparamConfig(clone.Gparam);
                    CollisionName = clone.CollisionName;
                    UnkT0C = clone.UnkT0C;
                    EnableObjAnimNetSyncStructure = clone.EnableObjAnimNetSyncStructure;
                    CollisionFilter = clone.CollisionFilter;
                    SetMainObjStructureBooleans = clone.SetMainObjStructureBooleans;
                    AnimIDs = (short[])clone.AnimIDs.Clone();
                    ModelSfxParamRelativeIDs = (short[])clone.ModelSfxParamRelativeIDs.Clone();
                }

                internal Object(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    CollisionPartIndex = br.ReadInt32();
                    UnkT0C = br.ReadByte();
                    EnableObjAnimNetSyncStructure = br.ReadBoolean();
                    CollisionFilter = br.ReadBoolean();
                    SetMainObjStructureBooleans = br.ReadBoolean();
                    AnimIDs = br.ReadInt16s(4);
                    ModelSfxParamRelativeIDs = br.ReadInt16s(4);
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(CollisionPartIndex);
                    bw.WriteByte(UnkT0C);
                    bw.WriteBoolean(EnableObjAnimNetSyncStructure);
                    bw.WriteBoolean(CollisionFilter);
                    bw.WriteBoolean(SetMainObjStructureBooleans);
                    bw.WriteInt16s(AnimIDs);
                    bw.WriteInt16s(ModelSfxParamRelativeIDs);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = MSB.FindName(entries.Parts, CollisionPartIndex);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    CollisionPartIndex = MSB.FindIndex(entries.Parts, CollisionName);
                }
            }

            /// <summary>
            /// Any non-player character, not necessarily hostile.
            /// </summary>
            public class Enemy : Part
            {
                internal override PartsType Type => PartsType.Enemy;

                internal override bool HasGparamConfig => true;
                internal override bool HasUnk4 => false;

                /// <summary>
                /// Gparam IDs for this enemy.
                /// </summary>
                public GparamConfig Gparam { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public string CollisionName { get; set; }
                private int CollisionPartIndex;

                /// <summary>
                /// Controls enemy AI.
                /// </summary>
                public int ThinkParamID { get; set; }

                /// <summary>
                /// Controls enemy stats.
                /// </summary>
                public int NPCParamID { get; set; }

                /// <summary>
                /// Controls enemy speech.
                /// </summary>
                public int TalkID { get; set; }

                /// <summary>
                /// Controls enemy equipment.
                /// </summary>
                public int CharaInitID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short ChrManipulatorAllocationParameter { get; set; }

                /// <summary>
                /// Walk route followed by this enemy.
                /// </summary>
                public string WalkRouteName { get; set; }
                private short WalkRouteIndex;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int BackupEventAnimID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT78 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT84;

                /// <summary>
                /// Creates a new Enemy with the given name.
                /// </summary>
                public Enemy(string name) : base(name)
                {
                    Gparam = new GparamConfig();
                }

                /// <summary>
                /// Creates a new Enemy with values copied from another.
                /// </summary>
                public Enemy(Enemy clone) : base(clone)
                {
                    Gparam = new GparamConfig(clone.Gparam);
                    ThinkParamID = clone.ThinkParamID;
                    NPCParamID = clone.NPCParamID;
                    TalkID = clone.TalkID;
                    UnkT04 = clone.UnkT04;
                    ChrManipulatorAllocationParameter = clone.ChrManipulatorAllocationParameter;
                    CharaInitID = clone.CharaInitID;
                    CollisionName = clone.CollisionName;
                    WalkRouteName = clone.WalkRouteName;
                    BackupEventAnimID = clone.BackupEventAnimID;
                    UnkT78 = clone.UnkT78;
                    UnkT84 = clone.UnkT84;
                }

                internal Enemy(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    ThinkParamID = br.ReadInt32();
                    NPCParamID = br.ReadInt32();
                    TalkID = br.ReadInt32();
                    UnkT04 = br.ReadInt16();
                    ChrManipulatorAllocationParameter = br.ReadInt16();
                    CharaInitID = br.ReadInt32();
                    CollisionPartIndex = br.ReadInt32();
                    WalkRouteIndex = br.ReadInt16();
                    br.AssertInt16(0);
                    br.AssertInt32(0);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    BackupEventAnimID = br.ReadInt32();
                    br.AssertInt32(-1); // BackupThrowAnimID
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    UnkT78 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    UnkT84 = br.ReadSingle();
                    for (int i = 0; i < 5; i++)
                    {
                        br.AssertInt32(-1);
                        br.AssertInt16(-1);
                        br.AssertInt16(0xA);
                    }
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ThinkParamID);
                    bw.WriteInt32(NPCParamID);
                    bw.WriteInt32(TalkID);
                    bw.WriteInt16(UnkT04);
                    bw.WriteInt16(ChrManipulatorAllocationParameter);
                    bw.WriteInt32(CharaInitID);
                    bw.WriteInt32(CollisionPartIndex);
                    bw.WriteInt16(WalkRouteIndex);
                    bw.WriteInt16(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(BackupEventAnimID);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(UnkT78);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteSingle(UnkT84);
                    for (int i = 0; i < 5; i++)
                    {
                        bw.WriteInt32(-1);
                        bw.WriteInt16(-1);
                        bw.WriteInt16(0xA);
                    }
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = MSB.FindName(entries.Parts, CollisionPartIndex);
                    WalkRouteName = MSB.FindName(msb.Events.WalkRoutes, WalkRouteIndex);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    CollisionPartIndex = MSB.FindIndex(entries.Parts, CollisionName);
                    WalkRouteIndex = (short)MSB.FindIndex(msb.Events.WalkRoutes, WalkRouteName);
                }
            }

            /// <summary>
            /// A player spawn point.
            /// </summary>
            public class Player : Part
            {
                internal override PartsType Type => PartsType.Player;

                internal override bool HasGparamConfig => false;
                internal override bool HasUnk4 => false;

                /// <summary>
                /// Creates a new Player with the given name.
                /// </summary>
                public Player(string name) : base(name) { }

                /// <summary>
                /// Creates a new Player with values copied from another.
                /// </summary>
                public Player(Player clone) : base(clone) { }

                internal Player(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// An invisible collision mesh, also used for death planes.
            /// </summary>
            public class Collision : Part
            {
                /// <summary>
                /// Amount of reverb to apply to sounds.
                /// </summary>
                public enum SoundSpace : byte
                {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                    NoReverb = 0,
                    SmallReverbA = 1,
                    SmallReverbB = 2,
                    MiddleReverbA = 3,
                    MiddleReverbB = 4,
                    LargeReverbA = 5,
                    LargeReverbB = 6,
                    ExtraLargeReverbA = 7,
                    ExtraLargeReverbB = 8,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
                }

                /// <summary>
                /// Unknown.
                /// </summary>
                public enum MapVisiblity : byte
                {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                    Good = 0,
                    Dark = 1,
                    PitchDark = 2,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
                }

                internal override PartsType Type => PartsType.Collision;

                internal override bool HasGparamConfig => true;
                internal override bool HasUnk4 => true;

                /// <summary>
                /// Gparam IDs for this collision.
                /// </summary>
                public GparamConfig Gparam { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct4 Unk4 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte HitFilterID { get; set; }

                /// <summary>
                /// Modifies sounds while the player is touching this collision.
                /// </summary>
                public SoundSpace SoundSpaceType { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short EnvLightMapSpotIndex { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float ReflectPlaneHeight { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short MapNameID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool DisableStart { get; set; }

                /// <summary>
                /// Disables a bonfire with this entity ID when an enemy is touching this collision.
                /// </summary>
                public int DisableBonfireEntityID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int PlayRegionID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short LockCamID1 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short LockCamID2 { get; set; }

                /// <summary>
                /// Unknown. Always refers to another collision part.
                /// </summary>
                public string UnkHitName { get; set; }
                private int UnkHitIndex;

                /// <summary>
                /// ID in MapMimicryEstablishmentParam.
                /// </summary>
                public int ChameleonParamID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT34 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT35 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT36 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public MapVisiblity MapVisType { get; set; }

                /// <summary>
                /// Creates a new Collision with the given name.
                /// </summary>
                public Collision(string name) : base(name)
                {
                    Gparam = new GparamConfig();
                    Unk4 = new UnkStruct4();
                    SoundSpaceType = SoundSpace.NoReverb;
                    MapNameID = -1;
                    DisableStart = false;
                    DisableBonfireEntityID = -1;
                    MapVisType = MapVisiblity.Good;
                    PlayRegionID = -1;
                }

                /// <summary>
                /// Creates a new Collision with values copied from another.
                /// </summary>
                public Collision(Collision clone) : base(clone)
                {
                    Gparam = new GparamConfig(clone.Gparam);
                    Unk4 = new UnkStruct4(clone.Unk4);
                    HitFilterID = clone.HitFilterID;
                    SoundSpaceType = clone.SoundSpaceType;
                    EnvLightMapSpotIndex = clone.EnvLightMapSpotIndex;
                    ReflectPlaneHeight = clone.ReflectPlaneHeight;
                    MapNameID = clone.MapNameID;
                    DisableStart = clone.DisableStart;
                    DisableBonfireEntityID = clone.DisableBonfireEntityID;
                    ChameleonParamID = clone.ChameleonParamID;
                    UnkHitName = clone.UnkHitName;
                    UnkT34 = clone.UnkT34;
                    UnkT35 = clone.UnkT35;
                    UnkT36 = clone.UnkT36;
                    MapVisType = clone.MapVisType;
                    PlayRegionID = clone.PlayRegionID;
                    LockCamID1 = clone.LockCamID1;
                    LockCamID2 = clone.LockCamID2;
                }

                internal Collision(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    HitFilterID = br.ReadByte();
                    SoundSpaceType = br.ReadEnum8<SoundSpace>();
                    EnvLightMapSpotIndex = br.ReadInt16();
                    ReflectPlaneHeight = br.ReadSingle();
                    br.AssertInt32(0); // Navmesh Group (4)
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(-1); // Vagrant Entity ID (3)
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    MapNameID = br.ReadInt16();
                    DisableStart = br.AssertInt16(0, 1) == 1;
                    DisableBonfireEntityID = br.ReadInt32();
                    ChameleonParamID = br.ReadInt32();
                    UnkHitIndex = br.ReadInt32();
                    UnkT34 = br.ReadByte();
                    UnkT35 = br.ReadByte();
                    UnkT36 = br.ReadByte();
                    MapVisType = br.ReadEnum8<MapVisiblity>();
                    PlayRegionID = br.ReadInt32();
                    LockCamID1 = br.ReadInt16();
                    LockCamID2 = br.ReadInt16();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);
                internal override void ReadUnk4(BinaryReaderEx br) => Unk4 = new UnkStruct4(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteByte(HitFilterID);
                    bw.WriteByte((byte)SoundSpaceType);
                    bw.WriteInt16(EnvLightMapSpotIndex);
                    bw.WriteSingle(ReflectPlaneHeight);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt16(MapNameID);
                    bw.WriteInt16((short)(DisableStart ? 1 : 0));
                    bw.WriteInt32(DisableBonfireEntityID);
                    bw.WriteInt32(ChameleonParamID);
                    bw.WriteInt32(UnkHitIndex);
                    bw.WriteByte(UnkT34);
                    bw.WriteByte(UnkT35);
                    bw.WriteByte(UnkT36);
                    bw.WriteByte((byte)MapVisType);
                    bw.WriteInt32(PlayRegionID);
                    bw.WriteInt16(LockCamID1);
                    bw.WriteInt16(LockCamID2);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);
                internal override void WriteUnk4(BinaryWriterEx bw) => Unk4.Write(bw);

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    UnkHitName = MSB.FindName(entries.Parts, UnkHitIndex);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    UnkHitIndex = MSB.FindIndex(entries.Parts, UnkHitName);
                }
            }

            /// <summary>
            /// An object that is either unused, or used for a cutscene.
            /// </summary>
            public class DummyObject : Object
            {
                internal override PartsType Type => PartsType.DummyObject;

                /// <summary>
                /// Creates a new DummyObject with the given name.
                /// </summary>
                public DummyObject(string name) : base(name) { }

                /// <summary>
                /// Creates a new DummyObject with values copied from another.
                /// </summary>
                public DummyObject(DummyObject clone) : base(clone) { }

                internal DummyObject(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// An enemy that is either unused, or used for a cutscene.
            /// </summary>
            public class DummyEnemy : Enemy
            {
                internal override PartsType Type => PartsType.DummyEnemy;

                /// <summary>
                /// Creates a new DummyEnemy with the given name.
                /// </summary>
                public DummyEnemy(string name) : base(name) { }

                /// <summary>
                /// Creates a new DummyEnemy with values copied from another.
                /// </summary>
                public DummyEnemy(DummyEnemy clone) : base(clone) { }

                internal DummyEnemy(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Determines which collision parts load other maps.
            /// </summary>
            public class ConnectCollision : Part
            {
                internal override PartsType Type => PartsType.ConnectCollision;

                internal override bool HasGparamConfig => false;
                internal override bool HasUnk4 => false;

                /// <summary>
                /// The name of the associated collision part.
                /// </summary>
                public string CollisionName { get; set; }
                private int CollisionIndex;

                /// <summary>
                /// The map to load when on this collision.
                /// </summary>
                public byte[] MapID { get; private set; }

                /// <summary>
                /// Creates a new ConnectCollision with the given name.
                /// </summary>
                public ConnectCollision(string name) : base(name)
                {
                    MapID = new byte[4];
                }

                /// <summary>
                /// Creates a new ConnectCollision with values copied from another.
                /// </summary>
                public ConnectCollision(ConnectCollision clone) : base(clone)
                {
                    CollisionName = clone.CollisionName;
                    MapID = clone.MapID;
                }

                internal ConnectCollision(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    CollisionIndex = br.ReadInt32();
                    MapID = br.ReadBytes(4);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(CollisionIndex);
                    bw.WriteBytes(MapID);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = MSB.FindName(msb.Parts.Collisions, CollisionIndex);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    CollisionIndex = MSB.FindIndex(msb.Parts.Collisions, CollisionName);
                }
            }
        }
    }
}