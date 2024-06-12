# Storage Firewall Admin

There are two (2) projects.

# FuncProjCS
This project is the C# Azure Function that is triggered when Comfiguration Metadata Storage Blob Chnages. 
Please make sure to have these two settings in local.settings.json and in App Setting in Azure Function
- "AzureWebJobsStorage": "Connection String for Azure Function Storage Account",
- "MetadataStoreConnection": "Connection String where Metadata Azure Blob is stored"
  
SampleData folder has the sample JSON file that you can use
Make sure to assign a Managed Identity to Function
Give Managed Idenity of the Function "Storage Blob Reader" access to the Metadata Storage Account 
Give Managed Idenity of the Function "Contributor" access to all the Storage accounts that you need to chnage permissions on

# StoreWFUICore
This project is a C# ASP.NET Core Razor project that allows you to manipulate Storage Firewall Metadat Blob
Please make sure to have these two settings in local.settings.json and in App Setting in Azure Function
- "MetadataStore": "Name of the Metadata Storage Account",
- "MetadataStoreContainer": "Name of the Metadata Storage Container"
- "MetadataStoreBlob": "Name of the Metadata Storage Blob"
  
Make sure to assign a Managed Identity to the Web App
Give Managed Idenity of the Web App "Storage Blob Contibutor" access to the Metadata Storage Account 


