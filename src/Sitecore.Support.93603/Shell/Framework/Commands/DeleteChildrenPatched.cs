using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using System.Linq;

namespace Sitecore.Support.Shell.Framework.Commands
{
  public class DeleteChildrenPatched : DeleteChildren
  {
    public override void Execute(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      Items.DeleteChildren(context.Items);
    }

    public override CommandState QueryState(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      return ((context.Items.Sum<Item>(item => Items.GetDeleteableChildren(item).Count) != 0) ? CommandState.Enabled : CommandState.Disabled);
    }

  }
}