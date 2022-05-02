using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Document_Blockchain.Models
{
    public class UploadBlockchainModel
    {
        [JsonProperty("pathFromS3")]
        public string PathFromS3 { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }




    public class DocusignModels
    {
    }
    public class HashModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("blockchainTrx")]
        public string BlockchainTrx { get; set; }

        [JsonProperty("ipfs")]
        public string IPFS { get; set; }

        [JsonProperty("blockchainId")]
        public string BlockchainId { get; set; }



    }

    public class SignedDocumentResponse
    {
        [JsonProperty("signedUsers")]
        public List<EmailModel> SignedUsers { get; set; }

        [JsonProperty("documentPath")]
        public string SignedDocumentURL { get; set; }

        [JsonProperty("signatureRequestedByEmail")]
        public string SignatureRequestedByEmail { get; set; }

        [JsonProperty("signatureRequestedByName")]
        public string SignatureRequestedByName { get; set; }
    }


    public class EnvelopesModel
    {
        [JsonProperty("recepients")]
        public List<EmailModel> Recepients { get; set; }

        [JsonProperty("documnetId")]
        public long DocumnetId { get; set; }
    }

    public class EnvelopeSentResponse
    {
        [JsonProperty("envelopeId")]
        public string EnvelopeId { get; set; }

        [JsonProperty("mailStatus")]
        public string MailStatus { get; set; }

        [JsonProperty("errorDetails")]
        public string ErrorDetails { get; set; }

        [JsonProperty("mailSentTime")]
        public DateTime MailSentTime { get; set; }

        [JsonProperty("documentPath")]
        public string DocumentURl { get; set; }

    }

    public class EnvelopeDetailsResponse
    {
        [JsonProperty("envelopeId")]
        public string EnvelopeId { get; set; }

        [JsonProperty("documentId")]
        public long DocumentId { get; set; }

        [JsonProperty("userDocumentId")]
        public long UserDocumentId { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("alternateName")]
        public string AlternateName { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("sentDocument")]
        public string SentDocument { get; set; }

        [JsonProperty("sentById")]
        public long SentById { get; set; }

        [JsonProperty("sentByName")]
        public string SentByName { get; set; }

        [JsonProperty("documentStatus")]
        public string DocumentStatus { get; set; }

        [JsonProperty("sentTime")]
        public DateTime SentTime { get; set; }

        [JsonProperty("documentPath")]
        public string SignedDocumentURL { get; set; }

        [JsonProperty("emails")]
        public List<SignedRecepients> Emails { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("signerName")]
        public string SignerName { get; set; }

        [JsonProperty("signerStatus")]
        public string SignerStatus { get; set; }

        [JsonProperty("signTime")]
        public string SignTime { get; set; }
    }

    public class EmailModel
    {
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("signerName")]
        public string SignerName { get; set; }
    }

    public class SignedRecepients
    {
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("signerName")]
        public string SignerName { get; set; }

        [JsonProperty("signerStatus")]
        public string SignatureStatus { get; set; }

        [JsonProperty("signatureTime")]
        public string SignatureTime { get; set; }

    }

}
