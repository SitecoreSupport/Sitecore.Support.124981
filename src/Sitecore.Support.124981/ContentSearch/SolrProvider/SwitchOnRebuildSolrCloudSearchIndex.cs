namespace Sitecore.Support.ContentSearch.SolrProvider
{
  using Sitecore.ContentSearch.Diagnostics;
  using Sitecore.ContentSearch.Maintenance;
  using Sitecore.ContentSearch.SolrProvider;
  using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
  using Sitecore.ContentSearch.SolrProvider.SolrOperations;
  using SolrNet;

  public class SwitchOnRebuildSolrCloudSearchIndex : Sitecore.ContentSearch.SolrProvider.SwitchOnRebuildSolrCloudSearchIndex
  {
    public SwitchOnRebuildSolrCloudSearchIndex(string name, string mainalias, string rebuildalias, string activecollection, string rebuildcollection, ISolrOperationsFactory solrOperationsFactory, IIndexPropertyStore propertyStore) 
      : base(name, mainalias, rebuildalias, activecollection, rebuildcollection, solrOperationsFactory, propertyStore)
    {
    }

    public override void Initialize()
    {
      // make sure index aliases created before index initialization is triggered as base.Initialize method makes call to index schema using aliases.
      if (SolrContentSearchManager.EnforceAliasCreation)
        SetAliasesConfiguration();
      base.Initialize();
    }

    protected virtual void SetAliasesConfiguration()
    {
      this.SetAlias(this.Core, this.ActiveCollection);
      this.SetAlias(this.RebuildCore, this.RebuildCollection);
    }

    protected new virtual void SetAlias(string aliasName, string collection)
    {
      ResponseHeader header = this.CreateAlias(aliasName, collection);
      if (header.Status == 0)
      {
        CrawlingLog.Log.Debug($"[Index={this.Name}] CreateAlias: Alias [{aliasName}] created/modified with collection [{collection}]", null);
      }
      else
      {
        CrawlingLog.Log.Error($"[Index={this.Name}] CreateAlias: Setting alias [{aliasName}] to collection [{collection}] failed. Response body: [Status={header.Status}, Params={header.Params}]", null);
      }
    }

    protected new virtual ResponseHeader CreateAlias(string alias, string collection)
    {
      var header = new ResponseHeader {Status = -1};
      if (SolrContentSearchManager.SolrAdmin != null && SolrContentSearchManager.SolrAdmin is ISolrCoreAdminEx)
      {
        header = ((ISolrCoreAdminEx) SolrContentSearchManager.SolrAdmin).CreateAlias(alias, collection);
      }
      return header;
    }
  }
}