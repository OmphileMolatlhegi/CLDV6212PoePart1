# CLDV6212PoePart1
web application deployment URL:
https://abcretail20250827220215.azurewebsites.net/  

Overview
ABC Retail is a modern cloud-based e-commerce management system built with ASP.NET Core MVC and Azure Cloud Services. The application addresses the challenges of traditional on-premises retail systems by leveraging Azure's scalable infrastructure for order processing, customer management, and inventory tracking.

Features
Core Functionality
Customer Management - Complete CRUD operations for customer data
Product Catalog - Manage products with images, pricing, and inventory
Order Processing - Create, edit, and track orders with real-time validation
File Upload System - Secure payment proof uploads to Azure Blob Storage
Dashboard Analytics - Real-time business metrics and reporting

Technical Features
Azure Table Storage - Scalable NoSQL data storage
Azure Blob Storage - Secure file storage for product images and documents
Real-time Validation - Stock level checking and pricing automation
Responsive Design - Mobile-friendly Bootstrap interface
Toastr Notifications - User-friendly feedback system

Architecture
Technology Stack
Frontend: ASP.NET Core MVC, Bootstrap 5, Font Awesome
Backend: .NET 8, C#
Database: Azure Table Storage
File Storage: Azure Blob Storage

Messaging: Azure Queue Storage (for future scaling)
Notifications: Toastr.js

Installation & Setup
Prerequisites
.NET 8 SDK
Azure Account with Storage Account
Visual Studio 2022 or VS Code

UI Components
Dashboard Features
Customer, Product, and Order statistics
Featured products display
Quick navigation buttons
Responsive card-based layout

Form Validation
Client-side validation with jQuery Validation
Server-side model validation
Real-time stock availability checking
File type validation for uploads

Performance Features
Async/Await patterns throughout
Connection pooling for Azure Storage
Client-side caching for static resources
Efficient data queries with Azure Table optimizations
Progress indicators for long-running operations

Security Features
Anti-forgery tokens on all forms
Input validation and sanitization
Secure file type validation
Azure SAS token protection for blobs
HTTPS enforcement in production

References & Documentation
Microsoft Documentation
Azure Table Storage
Azure Table Storage Overview: https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-overview
Azure Blob Storage
Azure Blob Storage Overview: https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction
Azure Queue Storage
Azure Queue Storage Overview: https://docs.microsoft.com/en-us/azure/storage/queues/storage-queues-introduction
ASP.NET Core MVC
Introduction to ASP.NET Core MVC: https://docs.microsoft.com/en-us/aspnet/core/mvc/overview 



