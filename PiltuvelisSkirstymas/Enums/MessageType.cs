using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiltuvelisSkirstymas.Enums
{
    public enum MessageType
    {
        Information = 0,
        Error = 1,

        /// <summary>
        /// Used to indicate that the message displayer will be reset. Neutral indication.
        /// </summary>
        Empty = 2
    }
}
