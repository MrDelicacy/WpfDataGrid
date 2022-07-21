namespace WpfDataGrid.ScaleConnection
{
    internal class StandartUpgradeString : IUpgradeString
    {
        public string Upgrade(string str)
        {
            string resulttxt = "";
            foreach (char ch in str)
            {
                if (ch == '+' || ch == '-' || ch == ' ' || ch == 'g'|| ch=='\r')
                    continue;
                if (ch == ',')
                    resulttxt += '.';
                else
                    resulttxt += ch.ToString();
            }
            return resulttxt;
        }
    }
}
