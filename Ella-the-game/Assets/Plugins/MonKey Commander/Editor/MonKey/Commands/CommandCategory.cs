using System.Collections.Generic;

namespace MonKey.Editor.Internal
{
    internal class CommandCategory
    {
        public bool IsMainCategory=>ParentCategoryName=="";
        public string CategoryName = "";
        public string ParentCategoryName="";
        public List<string> SubCategories=new List<string>();
        public List<string> CommandNames=new List<string>();

        public void AddCommandName(string name)
        {
            if (!CommandNames.Contains(name))
            {
                CommandNames.Add(name);
            }
        }

        public void AddSubCategory(string name)
        {
            if (!SubCategories.Contains(name))
            {
                SubCategories.Add(name);
            }
        }
    }
}