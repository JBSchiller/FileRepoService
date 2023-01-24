# FileRepoServiceApi
 
This is a simple file storage API that could be used for very basic revision management. Files are stored in an Sqlite database. The files are autmatically versioned when added or when a file with an existing name is modified.

# Solution

The solution contains an ASP.NET Core Web API project with driver console app that is setup to use the Swager API tool in development mode. Also included is a test project with a couple of very basic unut tests.

# Endpoints and Payloads

POST /api/v1/Repo/upload	- Adds a file or revision to the repo takes an IFormFile and path(string) for the file. Note that uploading a modified copy of an exisiting file will save the file with an automatically incremented version. Returns a Status200OK if successfull and Status400BadRequest if request failed

GET /api/v1/Repo/getall -	Returns a list of all files and associated info in the repository. There are no parameters. Returns a Status200OK and a list of files(FileItem) if successfull and Status400BadRequest if request failed

GET /api/v1/Repo/getfileVersion	- Returns a single file from the repository based on file name and version. This API requires the full filename(string) and version(float).  Returns a Status200OK with file(FileItem) if successfull and Status400BadRequest if request failed

GET /api/v1/Repo/getfile	- Returns the most recent version of a file from the repository based on file name. This API requires the full filename(string). Returns a Status200OK with file(FileItem) if successfull and Status400BadRequest if request failed.  

PUT /api/v1/Repo/modify	-	Updates an existing file based on file info(IFormFile), file path(string) and version(float). Returns a Status200OK if successfull and Status400BadRequest if request failed

DELETE /api/v1/Repo/Delete - Deletes a single file from the repository based on file name and version. This API requires the full filename(string) and version(float).  Returns a Status200OK if successfull and Status400BadRequest if request failed

DELETE /api/v1/Repo/SoftDelete - Flags a single file as deleted in the repository based on file name and version. This API requires the full filename(string) and version(float).  Returns a Status200OK if successfull and Status400BadRequest if request failed

DELETE /api/v1/Repo/PurgeDeletions - Deletes all files flagged as deleted from the repository. There are no parameters.  Returns a Status200OK and the number of files deleted(int) if successfull and Status400BadRequest if request failed

GET /api/v1/Repo/GetSoftDeletions - Returns all files flagged as deleted from the repository based on file name and version. There are no parameters.  Returns a Status200OK if successfull and Status400BadRequest if request failed

PUT /api/v1/Repo/RestoreDeletion - Restores a single file flagged as deleted to the repository based on file name and version. This API requires the full filename(string) and version(float).  Returns a Status200OK if successfull and Status400BadRequest if request failed

# Notes on Implmentation

The use of file versioning more or less eleminates the need to modify existing records in the db because a modified file generally results in new version of the file which results in a new record. That said there was a requirement in the task definition to provide update functionality so to accomdate this requirement this repository provides an update api that saves modified file contents for specified file versions.

File differencing is carried out by using checksum and revisions contain the entire content of the file. In a real world implmentation I would use a more sophisticated alogorithim to identify changes and store only the deltas.
