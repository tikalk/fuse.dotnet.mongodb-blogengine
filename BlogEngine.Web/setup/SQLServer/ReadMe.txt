﻿Running BlogEngine.NET 1.6 using SQL Server 2005/2008:

If you wish to use SQL Server to store all your blog data, this folder has all the 
information you'll likely need.  The scripts included here are for SQL Server 2005/2008.
They could be modified to be used with earlier or later versions if needed.

Included is the Initial Setup script for use with new installation of 1.6.  Also,
included is an upgrade script for users of BlogEngine 1.5.  In addition, you 
will find a sample web.config file with the needed changes to use SQL Server.

Instructions for new setup:

1. Open SQL Server Management Studio and connect to your SQL Server.
2. Create a new database if desired.
3. Execute the Setup script against the database you want to add the BlogEngine data to.
4. Rename SQLServerWeb.Config to Web.config and copy it to your blog folder.  (This will
overwrite your existing web.config file.  If this is not a new installation, make sure 
you have a backup.)
5. Update the BlogEngine connection string in the web.config.
6. Surf out to your Blog and see the welcome post.
7. Login with the username admin and password admin.  Change the password.

Upgrading from 1.5

1. Open SQL Server Management Studio and connect to your SQL Server.
2. Execute the desired upgrade script against the database where you have your BlogEngine
data.
3. The web.config file has changed from 1.5 to 1.6.  It will likely be easiest to start
with the sample web.config file as described above, but if you have other changes in it, 
you'll need to merge them.  Don't forget to move your connectionString over.

Additional information can be found at http://dotnetblogengine.net