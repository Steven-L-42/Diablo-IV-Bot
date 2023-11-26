using System.Collections.Generic;
using System.Linq;



namespace PixelAimbot.Classes.Misc
{
  
    internal class Priorized_Skills
    {
        public List<KeyValuePair<byte, int>> skillset { get; set; } = new Dictionary<byte, int>()
        {            
            {KeyboardWrapper.VK_1, 1},
            {KeyboardWrapper.VK_2, 2},
            {KeyboardWrapper.VK_3, 3},
            {KeyboardWrapper.VK_4, 4}
           
        }.ToList();
        
    }

}
