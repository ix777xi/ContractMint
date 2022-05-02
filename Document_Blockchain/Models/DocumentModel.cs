using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Document_Blockchain.Models
{
    public class RequestNewDocument
    {
        [JsonProperty("newDocumentDescription")]
        public string NewDocumentDescription { get; set; }

        [JsonProperty("category")]
        public long? Category { get; set; }
    }

    public class StatusDropDownModel
    {
        [JsonProperty("statusId")]
        public long Id { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }
    }



    public class ViewDocumentsByCategoryModel
    {

        [JsonProperty("CategoryId")]
        public long CategoryId { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }

    }

    public class ViewAllCategoriesModel
    {
        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }
    }
    public class EditRequestNewDocument
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("newDocumentDescription")]
        public string NewDocumentDescription { get; set; }

        [JsonProperty("categoryId")]
        public long? CategoryId { get; set; }
    }

    public class ViewNewDocumentRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("lastStatusUpdated")]
        public DateTime LastStatusUpdated { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("edit")]
        public string Edit { get; set; }
    }


    public class ViewDocumentRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("lastStatusUpdated")]
        public DateTime LastStatusUpdated { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("requestedBy")]
        public string RequestedBy { get; set; }

        [JsonProperty("requestedByEmail")]
        public string RequestedByEmail { get; set; }
    }


    public class Documentemails
    {
        public List<SignedRecepients> emails { get; set; }
    }

    public class CallApiModel
    {
        public string path { get; set; }
    }

    public class ResponseModel
    {
        public string status { get; set; }

        public string hash { get; set; }

        public string blockchainTrx { get; set; }

        public string ipfs { get; set; }

        public string blockchainId { get; set; }
    }

    public class DocumentModel
    {
    }
    public class NewDocument
    {
        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("document")]
        public string Document { get; set; }

        [JsonProperty("documentExtention")]
        public string DocumentExtention { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("gasFee")]
        public decimal GasFee { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }
    }

    public class AddEditedDocument
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("editedDocument")]
        public string EditedDocument { get; set; }

        [JsonProperty("extention")]
        public string Extention { get; set; }
    }


    public class AddHashDetailsModel
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


        [JsonProperty("documentId")]
        public long DocumentId { get; set; }
    }

    public class UpdateDocument
    {
        [JsonProperty("documentId")]
        public long DocumentId { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("document")]
        public string Document { get; set; }

        [JsonProperty("documentExtention")]
        public string DocumentExtention { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("gasFee")]
        public decimal GasFee { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }
    }

    public class AddCatModel
    {
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }
    }


    public class EditCatModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("categoryName")]
        public string Categoryname { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }

        [JsonProperty("extenction")]
        public string Extenction { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }
    }

}
