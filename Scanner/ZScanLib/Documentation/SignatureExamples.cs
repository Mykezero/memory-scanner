using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroLimits.ZScanLib
{
    public class SignatureExamples
    {
        class FFXIV_22
        {
        // Signature for player's CP / MP 
        public const string CP_SIGNATURE= "8908C6431D018B462C8B4E288945E4894DE085C074333BC8";
        public const string CP_MASK = "xxxxxxxxxxxxxxxxxxxxxxxx";
        public const int CP_OFFSET = 0x135D4AA6;

        // Alternative mask for CP / MP
        public const string CP_ALT_MASK_1 = "xxxxxxxxx";
        public const string CP_ALT_SIGNATURE_1 = "8908C6431D018B462C";
        public const int CP_ALT_OFFSET_1 = 0x11B14C86;
        }


        public class FFXIV23{
        
            /*
            00FDEED6   83F8 4A          CMP EAX,4A
            00FDEED9   73 0A            JNB SHORT ffxiv.00FDEEE5
            00FDEEDB   8B55 0C          MOV EDX,DWORD PTR SS:[EBP+C]
            00FDEEDE   899481 04010000  MOV DWORD PTR DS:[ECX+EAX*4+104],EDX
            00FDEEE5   5D               POP EBP
            00FDEEE6   C2 0800          RETN 8
            */

            //ffxiv Base ac0000 Size 12aa000 xxxxxxxxxxx????xxxx
            //83F84A730A8B550C899481000000005DC20800 

            public const string CP_SIGNATURE = "83F84A730A8B550C899481000000005DC20800";
            public const string CP_MASK = "xxxxxxxxxxx????xxxx";
            // 8 for offset with signature and 0xB25C32 for offset to cp value.
            public const int CP_OFFSET = 8 + 0xB25C32;

            /*
            01051117   894E 18          MOV DWORD PTR DS:[ESI+18],ECX
            0105111A   8858 1D          MOV BYTE PTR DS:[EAX+1D],BL
            ffxiv Base ac0000 Size 12aa000 \x89\x4E\x18\x88\x58\x1D xxxxxx
            */

            public const string CP_ALT_SIG = "894E1888581D";
            public const string CP_ALT_MASK = "xxxxxx";
            public const int CP_ALT_OFFSET = 0xCB576E1;

            public const string GAMEMAIN = "47616D654D61696E000000";
            public const string GAMEMASK = "xxxxxxxxxxx";
            public const int GAMEOFFSET = 1180;
        }
    }
}
