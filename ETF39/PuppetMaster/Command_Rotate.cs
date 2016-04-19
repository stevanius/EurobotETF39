﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace PuppetMaster
{
    public class Command_Rotate : Command
    {
        float ugao;
        Expression expr;

        public Command_Rotate(Controller c, float ugao) //degrees
            : base(c)
        {
            this.expr = null;
            this.ugao = ugao;
        }

        public Command_Rotate(Controller c, Expression expr)
            : base(c)
        {
            this.expr = expr;
            this.ugao = 0;
        }

        public override bool Send(CommBuffer buffer, out bool send, out bool free)
        {
            float val = (expr == null) ? ugao : expr.Value();

            // Metadata
            buffer.Write(0xFF);
            buffer.Write(0x0A);
            buffer.Write((byte)(8));

            // Data
            buffer.Write((byte)(val > 0 ? 0x06 : 0x07));
            //buffer.Write((byte)0x00);

            //int Ugao = (int)(Math.Abs(val) * 3100 / 90);
            int Ugao = (int)(Math.Abs(val) * c.r.rotationConst);
            
            buffer.Write((byte)(Ugao & 0x000F));
            buffer.Write((byte)((Ugao & 0x00F0) >> 4));
            buffer.Write((byte)((Ugao & 0x0F00) >> 8));
            buffer.Write((byte)((Ugao & 0xF000) >> 12));

            buffer.Write((byte)(ID & 0x0F));
            buffer.Write((byte)((ID & 0xF0) >> 4));

            return base.Send(buffer, out send, out free);
        }

        public static Command Parse(Controller c, string line)
        {
            string[] split = line.Split(' ');

            return new Command_Rotate(c, Expression.Parse(split[1]));

            //return new Command_Rotate(c, Convert.ToInt32(split[1]));
        }
    }
}
