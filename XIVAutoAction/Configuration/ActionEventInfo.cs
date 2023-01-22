namespace XIVAutoAttack.Configuration
{
    public class ActionEventInfo
    {
        public string Name { get; set; }
        public int MacroIndex { get; set; }
        public bool IsShared { get; set; }
        public bool IsEnable { get; set; }
        public ActionEventInfo()
        {
            Name = "";
            MacroIndex = -1;
            IsEnable = true;
            IsShared = false;
        }
    }
}
