using FSO.SimAntics.Model.TSOPlatform;
using System;
using System.IO;

namespace FSO.SimAntics.NetPlay.Model.Commands
{
    public class VMNetSetRoofCmd : VMNetCommandBodyAbstract
    {
        public float Pitch;
        public uint Style;

        public override bool Execute(VM vm, VMAvatar caller)
        {
            if (!vm.TS1 && (caller == null || caller.AvatarState.Permissions < VMTSOAvatarPermissions.Owner)) return false;
            if (Style >= Content.Content.Get().WorldRoofs.Count) return false;
            Pitch = Math.Max(0f, Math.Min(1.25f, Pitch));
            var arch = vm.Context.Architecture;
            // UI-42 tranche 2: roof clicks are undo steps (the native
            // cWinRoofPanel ×2 SubmitUndoable sites; r256 §6). The entry
            // stores the PRE-change pair and toggles on undo/redo.
            var roofEntry = new FSO.SimAntics.VMRoofUndoEntry
            {
                Pitch = arch.RoofPitch,
                Style = arch.RoofStyle,
                ActorUID = (caller != null) ? caller.PersistID : 0
            };
            arch.SetRoof(Pitch, Style);
            if (vm.TS1) arch.UndoStack.Submit(roofEntry);
            return true;
        }

        #region VMSerializable Members

        public override void SerializeInto(BinaryWriter writer)
        {
            base.SerializeInto(writer);
            writer.Write(Pitch);
            writer.Write(Style);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Pitch = reader.ReadSingle();
            Style = reader.ReadUInt32();
        }

        #endregion
    }
}
