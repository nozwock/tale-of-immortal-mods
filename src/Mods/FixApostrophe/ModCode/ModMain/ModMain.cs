namespace MOD_2UFdAZ
{
    public class ModMain
    {
        public void Init()
        {
            foreach (var item in g.conf.localText.allConfList)
            {
                item.ch = item.ch?.Replace("’", "'");
                item.tc = item.tc?.Replace("’", "'");
                item.en = item.en?.Replace("’", "'");
                item.kr = item.kr?.Replace("’", "'");
            }

            foreach (var item in g.conf.roleLogLocal.allConfList)
            {
                item.ch = item.ch?.Replace("’", "'");
                item.tc = item.tc?.Replace("’", "'");
                item.en = item.en?.Replace("’", "'");
                item.kr = item.kr?.Replace("’", "'");
            }
        }

        public void Destroy()
        {
        }
    }
}
