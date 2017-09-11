# PDFdocIcon

Use this solution to automatically include an icon for PDF documents in SharePoint 2010, 2013, or 2016. Here's a longer introduction on my [blog](https://thechriskent.com/2012/05/03/automatically-setting-up-pdf-icon-mapping-in-sharepoint-2010/).

## What it Does:
This solution does 2 things:
- Installs an Icon file in your Hive
- Adds a Mapping entry in the `TEMPLATE/XML/DOCICON.xml` file for PDF documents

## Benefits:
Users of your sites can often be confused by the fact that PDF documents in Document Libraries have a simple white icon like any other unrecognized file. By using this solution, the standard PDF icon will be shown instead making your users feel safe and secure as if in the hands of a fuzzy, cuddly bear.

All of this can be done manually as recommended by [Microsoft](http://support.microsoft.com/kb/2293357) and [Adobe](http://www.adobe.com/support/downloads/detail.jsp?ftpID=4025). However, in the event of recovery or the addition of new servers to your farm these changes will have to be repeated manually. This is both irritating and error-prone. By deploying these changes through a solution you can be sure that:
- The changes will be reapplied in the event of Disaster Recovery
- The changes will be applied to new servers as they are added to your farm
- You don't have to personally edit every 14 Hive for each server in your farm

## How it Works:
A small icon file is deployed to the file system on each server and on activation a one-time timer job runs. This job checks the `DOCICON.xml` file on each server for a pdf mapping and if it doesn't exist, it creates one pointing to the new icon.

An IIS reset is performed automatically as part of the deployment _(If the server running Central Admin is also hosting the web applications, it may need to have IIS reset manually)_. Then any document libraries with PDF files will now have the familiar PDF icon next to them! WHOO WHOO!

On deactivation, the same timer job runs again but this time removes the pdf mapping entry.


_The icon used comes from Adobe and can be found here: http://www.adobe.com/misc/linking.html_

> MIGRATED FROM CODEPLEX - This is an older, Farm based solution. It can still be used (especially with SharePoint 2010 & 2013) but only applies to On-Premises SharePoint (hence, the farm solution).
