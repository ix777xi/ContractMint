﻿// <auto-generated />
using System;
using Document_Blockchain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Document_Blockchain.Migrations
{
    [DbContext(typeof(BlockChainDbContext))]
    [Migration("20220328054550_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Document_Blockchain.Entities.Category", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_deleted");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_primary");

                    b.Property<string>("Name")
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<string>("PhotoPath")
                        .HasColumnType("longtext")
                        .HasColumnName("photo_path");

                    b.HasKey("Id");

                    b.ToTable("category");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.Country", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("CountryCode")
                        .HasColumnType("longtext")
                        .HasColumnName("coutry_code");

                    b.Property<string>("Name")
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("country");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.Document", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("Category_Id")
                        .HasColumnType("bigint")
                        .HasColumnName("category_id");

                    b.Property<string>("Description")
                        .HasColumnType("longtext")
                        .HasColumnName("description");

                    b.Property<string>("DocumentName")
                        .HasColumnType("longtext")
                        .HasColumnName("document_name");

                    b.Property<string>("DocumentPath")
                        .HasColumnType("longtext")
                        .HasColumnName("document_path");

                    b.Property<decimal>("GasFee")
                        .HasColumnType("decimal(65,30)")
                        .HasColumnName("gas_fee");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_deleted");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_primary");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(16,2)")
                        .HasColumnName("price");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("upload_date");

                    b.HasKey("Id");

                    b.HasIndex("Category_Id");

                    b.ToTable("documents");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.DocumentRequest", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long?>("CategoryId")
                        .HasColumnType("bigint")
                        .HasColumnName("category_id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<string>("DocumentDescription")
                        .HasColumnType("longtext")
                        .HasColumnName("document_description");

                    b.Property<long>("Status")
                        .HasColumnType("bigint")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_date");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("Status");

                    b.HasIndex("UserId");

                    b.ToTable("document_requests");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.DocumentSignerDetails", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("EnvelopeId")
                        .HasColumnType("bigint")
                        .HasColumnName("envelope_id");

                    b.Property<string>("SignatureStatus")
                        .HasColumnType("longtext")
                        .HasColumnName("signature_status");

                    b.Property<string>("SignatureTime")
                        .HasColumnType("longtext")
                        .HasColumnName("signature_time");

                    b.Property<string>("SignerEmail")
                        .HasColumnType("longtext")
                        .HasColumnName("signer_email");

                    b.Property<string>("SignerName")
                        .HasColumnType("longtext")
                        .HasColumnName("signer_name");

                    b.Property<long>("SignerRouting")
                        .HasColumnType("bigint")
                        .HasColumnName("signer_routing");

                    b.HasKey("id");

                    b.HasIndex("EnvelopeId");

                    b.ToTable("document_signer_details");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.DocusignAccessToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("AccessToken")
                        .HasColumnType("longtext")
                        .HasColumnName("accesstoken");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_time");

                    b.Property<DateTime>("ExpaireTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("expire_time");

                    b.Property<string>("TokenType")
                        .HasColumnType("longtext")
                        .HasColumnName("token_type");

                    b.HasKey("Id");

                    b.ToTable("docusign_accesstoken");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.EnvolopeDetails", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("DocumentId")
                        .HasColumnType("bigint")
                        .HasColumnName("document_id");

                    b.Property<string>("EnvelopeId")
                        .HasColumnType("longtext")
                        .HasColumnName("envelope_id");

                    b.Property<string>("ErrorDetails")
                        .HasColumnType("longtext")
                        .HasColumnName("error_details");

                    b.Property<string>("MailStatus")
                        .HasColumnType("longtext")
                        .HasColumnName("mail_status");

                    b.Property<long>("SentBy")
                        .HasColumnType("bigint")
                        .HasColumnName("sent_by");

                    b.Property<DateTime>("SentTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("sent_time");

                    b.Property<string>("SignedDocumentURL")
                        .HasColumnType("longtext")
                        .HasColumnName("signed_document_url");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("SentBy");

                    b.ToTable("envelope_detail");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.HashDetails", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("BlockchainId")
                        .HasColumnType("longtext")
                        .HasColumnName("blockchain_id");

                    b.Property<string>("BlockchainTrx")
                        .HasColumnType("longtext")
                        .HasColumnName("blockchain_trx");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<long>("DocumentId")
                        .HasColumnType("bigint")
                        .HasColumnName("document_id");

                    b.Property<string>("Hash")
                        .HasColumnType("longtext")
                        .HasColumnName("hash");

                    b.Property<string>("IPFS")
                        .HasColumnType("longtext")
                        .HasColumnName("ipfs");

                    b.Property<string>("Status")
                        .HasColumnType("longtext")
                        .HasColumnName("status");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("UserId");

                    b.ToTable("hash_details");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.PasswordResetToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("expiry_date");

                    b.Property<string>("Token")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("token");

                    b.Property<string>("Username")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.ToTable("password_reset_token");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.PaymentLedger", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint(20)")
                        .HasColumnName("id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(16,4)")
                        .HasColumnName("amount");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("last_updated_date");

                    b.Property<long>("OrderId")
                        .HasColumnType("bigint")
                        .HasColumnName("order_id");

                    b.Property<string>("TransactionStatus")
                        .HasColumnType("longtext")
                        .HasColumnName("transaction_status");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("payment_ledger");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.RefreshToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("expires");

                    b.Property<string>("Token")
                        .HasColumnType("longtext")
                        .HasColumnName("token");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("refresh_token");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.RequestStatusMaster", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Status")
                        .HasColumnType("longtext")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("request_status_master");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("role");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.SubscriptionToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<string>("PublishKey")
                        .HasColumnType("longtext")
                        .HasColumnName("publishKey");

                    b.Property<string>("SecretKey")
                        .HasColumnType("longtext")
                        .HasColumnName("secret_key");

                    b.HasKey("Id");

                    b.ToTable("subscription_token");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.TransactionLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(16,4)")
                        .HasColumnName("amount");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<long>("DocumentId")
                        .HasColumnType("bigint")
                        .HasColumnName("document_id");

                    b.Property<long>("OrderId")
                        .HasColumnType("bigint")
                        .HasColumnName("order_id");

                    b.Property<string>("OrderStatus")
                        .HasColumnType("longtext")
                        .HasColumnName("order_status");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("transaction_log");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("ContactNumber")
                        .HasColumnType("bigint")
                        .HasColumnName("contact_number");

                    b.Property<long?>("CountryId")
                        .HasColumnType("bigint")
                        .HasColumnName("country_id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<string>("EmailId")
                        .HasColumnType("longtext")
                        .HasColumnName("email_Id");

                    b.Property<string>("FullName")
                        .HasColumnType("longtext")
                        .HasColumnName("full_name");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<bool?>("Is_EmailVerified")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_emailverified");

                    b.Property<bool>("Isadmin")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_admin");

                    b.Property<string>("Password")
                        .HasColumnType("longtext")
                        .HasColumnName("password");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("longtext")
                        .HasColumnName("profile_picture");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("user");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserCart", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<long>("DocumentId")
                        .HasColumnType("bigint")
                        .HasColumnName("document_id");

                    b.Property<string>("OrderStatus")
                        .HasColumnType("longtext")
                        .HasColumnName("order_status");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_date");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.Property<long?>("UserOrderId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_order_id");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserOrderId");

                    b.ToTable("user_cart");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserDocuments", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("DocumentId")
                        .HasColumnType("bigint")
                        .HasColumnName("document_id");

                    b.Property<string>("EditedDocument")
                        .HasColumnType("longtext")
                        .HasColumnName("edited_document");

                    b.Property<bool?>("IsEdited")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_edited");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("order_date");

                    b.Property<long>("OrderId")
                        .HasColumnType("bigint")
                        .HasColumnName("order_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<int>("RemainingQuantity")
                        .HasColumnType("int")
                        .HasColumnName("remaining_quantity");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_date");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("user_documents");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserOrders", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<decimal>("OrderAmount")
                        .HasColumnType("decimal(16,4)")
                        .HasColumnName("order_amount");

                    b.Property<string>("OrderSatus")
                        .HasColumnType("longtext")
                        .HasColumnName("order_status");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_date");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("user_order");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasColumnName("role_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("user_role");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.Document", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Category", "Category")
                        .WithMany("Document")
                        .HasForeignKey("Category_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.DocumentRequest", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Category", "Category")
                        .WithMany("DocumentRequest")
                        .HasForeignKey("CategoryId");

                    b.HasOne("Document_Blockchain.Entities.RequestStatusMaster", "RequestStatusMaster")
                        .WithMany("DocumentRequest")
                        .HasForeignKey("Status")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("DocumentRequest")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("RequestStatusMaster");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.DocumentSignerDetails", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.EnvolopeDetails", "EnvolopeDetails")
                        .WithMany("DocumentSignerDetails")
                        .HasForeignKey("EnvelopeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EnvolopeDetails");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.EnvolopeDetails", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Document", "Document")
                        .WithMany("EnvolopeDetails")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("EnvolopeDetails")
                        .HasForeignKey("SentBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.HashDetails", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Document", "Document")
                        .WithMany("HashDetails")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("HashDetails")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.PaymentLedger", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.UserOrders", "UserOrders")
                        .WithMany("PaymentLedger")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("PaymentLedger")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("UserOrders");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.RefreshToken", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("RefreshToken")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.TransactionLog", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Document", "Document")
                        .WithMany("TransactionLog")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.UserOrders", "UserOrders")
                        .WithMany("TransactionLog")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("TransactionLog")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("User");

                    b.Navigation("UserOrders");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.User", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Country", "Country")
                        .WithMany("User")
                        .HasForeignKey("CountryId");

                    b.Navigation("Country");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserCart", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Document", "Document")
                        .WithMany("UserCart")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("UserCart")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.UserOrders", "UserOrders")
                        .WithMany("UserCart")
                        .HasForeignKey("UserOrderId");

                    b.Navigation("Document");

                    b.Navigation("User");

                    b.Navigation("UserOrders");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserDocuments", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Document", "Document")
                        .WithMany("UserDocuments")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.UserOrders", "UserOrders")
                        .WithMany("UserDocuments")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("UserDocuments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("User");

                    b.Navigation("UserOrders");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserOrders", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("UserOrders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserRole", b =>
                {
                    b.HasOne("Document_Blockchain.Entities.Role", "Role")
                        .WithMany("UserRole")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Document_Blockchain.Entities.User", "User")
                        .WithMany("UserRole")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.Category", b =>
                {
                    b.Navigation("Document");

                    b.Navigation("DocumentRequest");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.Country", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.Document", b =>
                {
                    b.Navigation("EnvolopeDetails");

                    b.Navigation("HashDetails");

                    b.Navigation("TransactionLog");

                    b.Navigation("UserCart");

                    b.Navigation("UserDocuments");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.EnvolopeDetails", b =>
                {
                    b.Navigation("DocumentSignerDetails");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.RequestStatusMaster", b =>
                {
                    b.Navigation("DocumentRequest");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.Role", b =>
                {
                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.User", b =>
                {
                    b.Navigation("DocumentRequest");

                    b.Navigation("EnvolopeDetails");

                    b.Navigation("HashDetails");

                    b.Navigation("PaymentLedger");

                    b.Navigation("RefreshToken");

                    b.Navigation("TransactionLog");

                    b.Navigation("UserCart");

                    b.Navigation("UserDocuments");

                    b.Navigation("UserOrders");

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("Document_Blockchain.Entities.UserOrders", b =>
                {
                    b.Navigation("PaymentLedger");

                    b.Navigation("TransactionLog");

                    b.Navigation("UserCart");

                    b.Navigation("UserDocuments");
                });
#pragma warning restore 612, 618
        }
    }
}
