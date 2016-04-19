﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace PuppetMaster
{
    public class Command_SetPrescaler : Command
    {
        float prescaler;

        public Command_SetPrescaler(Controller c, float prescaler)
            : base(c)
        {
            this.prescaler = prescaler;
        }

        public override bool Send(CommBuffer buffer, out bool send, out bool free)
        {
            int data = (int)prescaler;

            // Metadata
            buffer.Write(0xFF);
            buffer.Write(0x0A);
            buffer.Write(6);

            // Data
            buffer.Write((byte)(0xFB));
            //buffer.Write((byte)(val >= 0 ? 0xF1 : 0xF2));
            //buffer.Write((byte)0x00);
            
            buffer.Write((byte)(data & 0x000F));
            buffer.Write((byte)((data & 0x00F0) >> 4));
            buffer.Write((byte)((data & 0x0F00) >> 8));
            buffer.Write((byte)((data & 0xF000) >> 12));

            return base.Send(buffer, out send, out free);
        }

        public static Command Parse(Controller c, string line)
        {
            string[] split = line.Split(' ');

            return new Command_SetPrescaler(c, Convert.ToInt32(split[1]));
        }
    }
}
