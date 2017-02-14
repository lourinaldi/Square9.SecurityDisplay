using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Square9.SecurityDisplay.Models
{
    public class Enumerations
    {
        public enum Property
        {
            ViewDocuments = 1,
            AddNewDocuments = 2,
            ModifyDocuments = 4,
            DeleteDocuments = 8,
            PrintDocuments = 16,
            EmailDocuments = 32,
            ExportData = 64,
            ExportDocuments = 128,
            ViewInAcrobat = 256,
            ViewDocumentHistory = 512,
            ModifyData = 1024,
            ModifyAnnotations = 2048,
            LaunchDocument = 4096,
            LaunchNewVersion = 8192,
            ViewDocumentRevisions = 16384,
            PublishDocumentRevisions = 32768,
            ModifyDocumentPages = 65536,
            MoveDocuments = 131072,
            DeleteBatches = 262144,
            APIFullAccess = 524288
        }

        [Flags]
        public enum DatabaseSecurity
        {
            None = 0,
            Admin = 1,
            DeleteBatches = 2
        }

        [Flags]
        public enum ArchiveSecurityType
        {
            Group = 0,
            User = 1,
            Search = 2,
            Queue = 4,
            Default = 8,
            Direct = 16
        }

        public enum SecurityLevel
        {
            SSAdmin,
            Admin,
            AdminAll,
            AdminAny
        }

        public enum ArchiveProperty
        {
            RevisionControl = 1,
            PdfConvert = 2,
            WebRevision = 4,
            ReadOnly = 8,
            VersionsArchive = 16
        }

        public enum SearchOperator
        {
            Equals = 1,
            Contains = 2,
            GreaterThanEqual = 3,
            LessThanEqual = 4,
            DoesNotEqual = 5,
            IsEmpty = 6,
            IsNotEmpty = 7
        }

        [Flags]
        public enum SearchProperty
        {
            ContentSearch = 1,
            Phonics = 2,
            Stemming = 4,
            IXEnabled = 8,
            MultiValueSearch = 16,
            DisplayViewTabs = 32
        }

        public enum FieldProperty
        {
            QuickSearch = 1,
            Required = 2,
            DateEntered = 4,
            IndexedBy = 8,
            PageCount = 16,
            MVField = 32,
            ContainsList = 64,
            ContainsDynamicList = 128,
            LastModifiedBy = 256,
            TableField = 512,
            FileType = 1024,
            ReadOnly = 2048,
            DocumentVersion = 4096,
            DocumentParent = 8192
        }

        public enum FieldDataType
        {
            Character = 1,
            Numeric = 2,
            Date = 3,
            Decimal = 4
        }
    }
}