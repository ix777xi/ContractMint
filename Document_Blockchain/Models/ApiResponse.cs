using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Document_Blockchain.Models
{
    public class ViewCart
    {
        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("userViewCartId")]
        public long UserViewCartId { get; set; }

        [JsonProperty("documentId")]
        public long DocumentId { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("documentPhotoPath")]
        public string DocumentPhotoPath { get; set; }

        [JsonProperty("gasFee")]
        public decimal GasFee { get; set; }


    }


    public class ViewAdminDocument
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("documentPath")]
        public string DocumentPath { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("gasFee")]
        public decimal GasFee { get; set; }

        [JsonProperty("categoryId")]
        public long CategoryId { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

    }





    public class ViewDocument
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("documentId")]
        public long DocumentId { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("alternateName")]
        public string AlternateName { get; set; }

        [JsonProperty("documentPath")]
        public string DocumentPath { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("gasFee")]
        public decimal GasFee { get; set; }

        [JsonProperty("categoryId")]
        public long CategoryId { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("signedRecepients")]
        public List<SignedRecepients> SignedRecepients { get; set; }

        [JsonProperty("signatureStatus")]
        public string SignatureStatus { get; set; }

        [JsonProperty("signedDocumentUrl")]
        public string SignedDocumentUrl { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("hashDetails")]
        public HashModel HashDetails { get; set; }

        [JsonProperty("isEdited")]
        public bool? IsEdited { get; set; }

        [JsonProperty("newDocument")]
        public string NewDocument { get; set; }
    }

    public class Categories
    {
        [JsonProperty("categoryId")]
        public long CategoryId { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("photoPath")]
        public string PhotoPath { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }
    }

    public class ResendDocumentModel
    {
        [JsonProperty("documentId")]
        public long DocumentId { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("gasFee")]
        public decimal GasFee { get; set; }
    }

    public class ConfirmResentModel
    {
        [JsonProperty("DocumentId")]
        public long DocumentId { get; set; }

        [JsonProperty("alternateName")]
        public string AlternateName { get; set; }
    }


    public class ApiResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("statusCode")]
        public long StatusCode { get; set; }

        public ApiResponse(string Message, int statusCode)
        {
            this.Message = Message;
            this.StatusCode = statusCode;
        }
    }

    public class ServiceResponse<T>
    {
        public ServiceResponse(long StatusCode, string Message, T Data)
        {
            this.Message = Message;
            this.StatusCode = StatusCode;
            this.Data = Data;
        }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("statusCode")]
        public long StatusCode { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

}
public class ApiResponse<T>
{
    public ApiResponse(long StatusCode, string Message, T Data)
    {
        this.Message = Message;
        this.StatusCode = StatusCode;
        this.Data = Data;
    }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("statusCode")]
    public long StatusCode { get; set; }

    [JsonProperty("data")]
    public T Data { get; set; }
}

public class PagedResponse<T>
{
    [JsonProperty("pageIndex")]
    public int PageIndex { get; private set; }

    [JsonProperty("totalPages")]
    public int TotalPages { get; private set; }

    [JsonProperty("totalRecords")]
    public int TotalRecords { get; private set; }

    [JsonProperty("data")]
    public IEnumerable<T> Data { get; private set; }

    public PagedResponse(List<T> items, int count, int pageIndex, int pageSize)
    {
        this.PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalRecords = count;
        Data = items;
    }

    public bool hasPreviousPage
    {
        get
        {
            return (PageIndex > 0);
        }
    }

    public bool hasNextPage
    {
        get
        {
            return ((PageIndex + 1) < TotalPages);
        }
    }

    public static PagedResponse<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        return new PagedResponse<T>(items, count, pageIndex, pageSize);
    }

    public static async Task<PagedResponse<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        return new PagedResponse<T>(items, count, pageIndex, pageSize);
    }

    public static PagedResponse<T> Create(IEnumerable<T> source, int pageIndex, int pageSize, int totalRecords)
    {
        var count = source.Count();
        var items = source.ToList();
        return new PagedResponse<T>(items, totalRecords, pageIndex, pageSize);
    }
}

