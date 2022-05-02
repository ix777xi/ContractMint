using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Document_Blockchain.Constants
{
    public class StringConstants
    {
    }

    public class ApiResponseConstants
    {
        public const string invalidInput = "Invalid Input";
        public const string refrestTokenMandatory = "refrestToken parameter is mandatory";
        public const string notAllSignersSigned = "One or More Previous Signers have Not Signed the Document";
        public const string mailAlreadySent = "mail is already sent to the email";
        public const string success = "success";
        public const string envelopeDoesNotExists = "envelope doesnot exists";
        public const string completed = "completed";
        public const string alreadyUploaded = "Document already uploaded to BlockChain";
        public const string hashGenerated = "Hash Generated";
        public const string hashNotGenerated = "Hash Not Generated";

    }

    public class DocusignConstants
    {
        public const string emailSubject = "Please sign this document sent";
        public const string defaultDocumentName = "Please sign this document sent";
        public const string Signed = "Signed";
        public const string NotSigned = " is not signed";
        public const string AllSigned = "all recepents have signed";

    }

    public class EmailStatus
    {
        public const string sent = "Sent";
        public const string pending = "Pending";
    }

    public class DocuSignScopes
    {
        public const string impersonation = "impersonation";
        public const string signature = "signature";
    }

    public class MediaExtentions
    {
        public const string pdf = ".pdf";
        public const string jpeg = ".jpeg";

    }

    public class NumaricsAsStrings
    {
        public const string _1 = "1";
        public const string _2 = "2";
        public const string _3 = "3";
        public const string _4 = "4";
        public const string _5 = "5";
        public const string _6 = "6";
        public const string _7 = "7";
        public const string _8 = "8";
        public const string _9 = "9";
        public const string _0 = "0";


    }
}
