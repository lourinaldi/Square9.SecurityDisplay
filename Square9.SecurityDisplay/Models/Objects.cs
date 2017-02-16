using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Square9.SecurityDisplay.Models
{

    public class Permissions
    {
        public int DatabaseID { get; set; }
        public string DatabaseName { get; set; }
        public int ArchiveID { get; set; }
        public string ArchiveName { get; set; }
        public string Group { get; set; }
        public string User { get; set; }
        public FolderLevel FolderLevel { get; set; }
        public DocumentLevel DocumentLevel { get; set; }
        public ExportLevelSecurity ExportLevelSecurity { get; set; }
        public SearchSecurity SearchSecurity { get; set; }

        public Permissions()
        {
            FolderLevel = new FolderLevel();
            DocumentLevel = new DocumentLevel();
            ExportLevelSecurity = new ExportLevelSecurity();
            SearchSecurity = new SearchSecurity();
        }
    }

    public class FolderLevel
    {
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Delete { get; set; }
        public bool Move { get; set; }
        public bool ViewRevisions { get; set; }
        public bool ViewHistory { get; set; }
        public bool DeleteErroredBatches { get; set; }
        public bool APIFullAccess { get; set; }

        public FolderLevel()
        {
            View = false;
            Add = false;
            Delete = false;
            Move = false;
            ViewRevisions = false;
            ViewHistory = false;
            DeleteErroredBatches = false;
            APIFullAccess = false;
        }
    }

    public class DocumentLevel
    {
        public bool ModifyDocument { get; set; }
        public bool ModifyPages { get; set; }
        public bool ModifyData { get; set; }
        public bool ModifyAnnotations { get; set; }
        public bool PublishRevisions { get; set; }
    }

    public class ExportLevelSecurity
    {
        public bool Print { get; set; }
        public bool Email { get; set; }
        public bool ExportDocument { get; set; }
        public bool ExportData { get; set; }
        public bool ViewInAcrobat { get; set; }
        public bool Launch { get; set; }
        public bool LaunchCopy { get; set; }
    }

    public class SearchSecurity
    {
        public string Searches { get; set; }
        public string DeafaultArchiveSearch { get; set; }
        public string DefaultAccessSearch { get; set; }
    }

    public class Database
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Int32 SecurityLevel { get; set; }
        public String Manager { get; set; }
    }

    [DataContract()]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Square9.Objects")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.datacontract.org/2004/07/Square9.Objects", IsNullable = false)]
    public class SecuredGroup
    {
        [DataMember()]
        public String Name { get; set; }
        [DataMember()]
        public String Email { get; set; }
        [DataMember()]
        public Int32 License { get; set; }
        [DataMember()]
        public Int32 Type { get; set; }
        [DataMember()]
        public List<Database> SecuredDBs { get; set; }
    }

    [DataContract()]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Square9.Objects")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.datacontract.org/2004/07/Square9.Objects", IsNullable = false)]
    public class License
    {
        [DataMember()]
        public String Username { get; set; }
        [DataMember()]
        public String Domain { get; set; }
        [DataMember()]
        public String AuthServer { get; set; }
        [DataMember()]
        public Int32 Type { get; set; }
        [DataMember()]
        public String IPAddress { get; set; }
        [DataMember()]
        public DateTime DateCreated { get; set; }
        [DataMember()]
        public DateTime DateAccessed { get; set; }
        [DataMember()]
        public String Token { get; set; }
        [DataMember()]
        public Int32 Reg { get; set; }
    }

    public class Archive
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Int32 Parent { get; set; }
        public Int32 Permissions { get; set; }
        public Int32 Properties { get; set; } //Bitwise flag property for ArchiveProperties enum
        public Int32 DBProperties { get; set; }
        public String BasePath { get; set; }
        public String FullTextPath { get; set; }
    }

    [DataContract()]
    public class SearchArchive
    {
        [DataMember()]
        public Int32 ID { get; set; }
        [DataMember()]
        public String Name { get; set; }
    }

    [DataContract()]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Square9.Objects")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.datacontract.org/2004/07/Square9.Objects", IsNullable = false)]
    public class Search
    {
        [DataMember()]
        public Int32 Id { get; set; }
        [DataMember()]
        public Int32 Parent { get; set; }
        [DataMember()]
        public String Name { get; set; }
        [DataMember()]
        public String Hash { get; set; }
        [DataMember()]
        public List<SearchArchive> Archives = new List<SearchArchive>();
        [DataMember()]
        public List<SearchDetail> Detail { get; set; }
        [DataMember()]
        public Int32 Props { get; set; }
        [DataMember()]
        public Int32 Fuzzy { get; set; }
        [DataMember()]
        public String Grouping { get; set; }
        [DataMember()]
        public Int32 Settings { get; set; }
    }

    [DataContract]
    public class SecurityNode
    {
        [DataMember]
        public Int32 Id { get; set; }
        [DataMember]
        public Int32 DbId { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public bool DefaultSearch { get; set; }
        [DataMember]
        public bool QueueSearch { get; set; }
        [DataMember]
        public bool DirectSearch { get; set; }
        [DataMember]
        public List<SecurityNode> Children { get; set; }

        public SecurityNode()
        {

        }

        public SecurityNode(Database database)
        {
            this.Id = database.Id;
            this.DbId = database.Id;
            this.Label = database.Name;
            this.Type = "database";
        }

        public SecurityNode(Archive archive, Int32 dbId)
        {
            this.Id = archive.Id;
            this.DbId = dbId;
            this.Label = archive.Name;
            this.Type = "archive";
        }

        public SecurityNode(Search search, Int32 dbId)
        {
            this.Id = search.Id;
            this.DbId = dbId;
            this.Label = search.Name;
            this.Type = "search";
            this.DefaultSearch = (search.Props | (Int32)Enumerations.ArchiveSecurityType.Default) > 0;
            this.QueueSearch = (search.Props | (Int32)Enumerations.ArchiveSecurityType.Queue) > 0;
            this.DirectSearch = (search.Props | (Int32)Enumerations.ArchiveSecurityType.Direct) > 0;
        }
    }

    [DataContract()]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Square9.Objects")]
    public class SearchDetail
    {
        [DataMember()]
        public Int32 ID { get; set; }
        [DataMember()]
        public Int32 FID { get; set; }
        [DataMember()]
        public Int32 ListID { get; set; }
        [DataMember()]
        public Int32 ListF1 { get; set; }
        [DataMember()]
        public Int32 ListF2 { get; set; }
        [DataMember()]
        public Int32 Parent { get; set; }
        [DataMember()]
        public Int32 Operator { get; set; }
        [DataMember()]
        public String Prompt { get; set; }
        [DataMember()]
        public String VAL { get; set; }
        [DataMember()]
        public Int32 Prop { get; set; }
        [DataMember()]
        public Int32 FieldType { get; set; }
        [DataMember()]
        public String Mask { get; set; }
    }
}