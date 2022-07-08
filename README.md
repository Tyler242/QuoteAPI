# QuoteAPI

Base URL: https://quoteapp-api.herokuapp.com/api

## Current Endpoints

* GET /quotes
	Returns a list containing every quote within the database.
* POST /quotes
	Creates a quote from the quote object that is sent with the request.
* GET /quotes/{id}
	Returns a single quote that matches the id sent with the url.

## Future Work

* Add DELETE and PUT endpoints for individual quotes.
* Add query parameters on GET /quotes to allow filtering.
* Add authentication to allow for multiple users with separate tables.

## Tools

* C#
* ASP.NET Framework
* MongoDB
* GitHub
* Heroku
* [Dotnetcore-buildpack](https://elements.heroku.com/buildpacks/jincod/dotnetcore-buildpack)