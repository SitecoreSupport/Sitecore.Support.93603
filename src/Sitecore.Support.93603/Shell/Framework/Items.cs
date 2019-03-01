using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Sitecore.Support.Shell.Framework
{
  public class Items
  {
    public Items()
    {
    }

    public static void DeleteChildren(Item[] items)
    {
      Assert.ArgumentNotNull(items, "items");
      Items.DeleteChildren(items, string.Empty);
    }

    public static void DeleteChildren(Item[] items, string message)
    {
      Assert.ArgumentNotNull(items, "items");
      Assert.ArgumentNotNull(message, "message");
      if (items.Length != 0)
      {
        List<Item> itemList = new List<Item>();
        for (int i = 0; i < items.Length; i++)
        {
          Item item = items[i];
          var childrenToRemove = GetDeleteableChildren(item);
          if (childrenToRemove.Count > 0)
          {
            itemList.AddRange(childrenToRemove);
          }
        }
        if (itemList.Count == 0)
        {
          SheerResponse.Alert("You do not have permissions to delete any of the subitems.", new string[0]);
          return;
        }
        if (itemList.Count != items.Count())
        {
          SheerResponse.Alert($"You do not have permissions to delete some of the subitems.", new string[0]);
        }
        Items.Start("uiDeleteItems", items[0].Database, itemList.ToArray())["message"] = message;
      }
    }

    public static ItemList GetDeleteableChildren(Item item)
    {
      Item[] items = { item };
      return GetDeleteableChildren(items);
    }
    public static ItemList GetDeleteableChildren(Item[] items)
    {
      Assert.ArgumentNotNull(items, "items");
      ItemList itemList = new ItemList();
      foreach (Item item in items)
      {
        Assert.ArgumentNotNull(item, "item");
        foreach (Item child in item.GetChildren())
        {
          if (child.Access.CanRead() && child.Access.CanDelete())
          {
            itemList.Add(child);
          }
        }
      }

      return itemList;
    }

    private static NameValueCollection Start(string pipelineName, Database database, Item[] items)
    {
      Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(database, "database");
      Assert.ArgumentNotNull(items, "items");
      ClientPipelineArgs clientPipelineArg = new ClientPipelineArgs();
      Items.Start(pipelineName, clientPipelineArg, database, items);
      return clientPipelineArg.Parameters;
    }

    private static NameValueCollection Start(string pipelineName, ClientPipelineArgs args, Database database, Item[] items)
    {
      Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(database, "database");
      Assert.ArgumentNotNull(items, "items");
      return Items.Start(pipelineName, args, database, items, null);
    }

    private static NameValueCollection Start(string pipelineName, ClientPipelineArgs args, Database database, Item[] items, NameValueCollection additionalParameters)
    {
      Assert.ArgumentNotNull(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(database, "database");
      Assert.ArgumentNotNull(items, "items");
      Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
      NameValueCollection nameValueCollection = new NameValueCollection();
      ListString listString = new ListString('|');
      Item[] itemArray = items;
      for (int i = 0; i < (int)itemArray.Length; i++)
      {
        Item item = itemArray[i];
        listString.Add(item.ID.ToString());
      }
      string str = items[0].Language.ToString();
      nameValueCollection.Add("database", database.Name);
      nameValueCollection.Add("items", listString.ToString());
      nameValueCollection.Add("language", str);
      args.Parameters = nameValueCollection;
      if (additionalParameters != null)
      {
        string[] allKeys = additionalParameters.AllKeys;
        for (int j = 0; j < (int)allKeys.Length; j++)
        {
          string item1 = allKeys[j];
          args.Parameters[item1] = additionalParameters[item1];
        }
      }
      Context.ClientPage.Start(pipelineName, args);
      return nameValueCollection;
    }
  }
}