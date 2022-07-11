# QuoteAPI

Base URL: https://quoteapp-api.herokuapp.com/api

## Current Endpoints

* GET /quotes?word={word}&source={source}&tag={tag}
	Returns a list containing quotes that match the query parameter filters.
* POST /quotes
	Creates a quote from the quote object that is sent with the request.
* GET /quotes/{id}
	Returns a single quote that matches the id sent with the url.
* PUT /quotes/{id}
	Updates a single quote that matches the id sent with the url.
* DELETE /quotes/{id}
	Deletes a single quote that matches the id sent wiht the url.

## Future Work

* Add authentication to allow for multiple users with separate tables.

## Tools

* C#
* ASP.NET Framework
* MongoDB
* GitHub
* Heroku
* [Dotnetcore-buildpack](https://elements.heroku.com/buildpacks/jincod/dotnetcore-buildpack)