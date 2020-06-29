# amber-music

Demo Application for finding the average number of words in an artist's song by consuming various APIs and aggregating data.

Distributables for the apps can be found in the Runnables folder (Runnables.zip) for ease of running the product. 
I meant to have the result distributed as containers, but had some CORS issues and I left it for another time as I've ran out of time.
I've also changed the communication protocol to be on http rather than https to avoid any fun with missing certificates.

## How to run?

### From the executables

There are two folders in the Runnables one

API:  
- Requirements dotnet Core 3.1  
- Run Amber.Music.Api.exe or dotnet Amber.Music.Api.dll (starts on http:5000)  
	
UI:  
- Install Angular's http server to be able to server the compiled code  
- ```npm install angular-http-server -g```  
- from the context of the UI folder run the following command to start a the application and open a browser window
- ```angular-http-server --path . -p 4200 --open```

### From the codebase

In case you want to run the application you can do so via Visual Studio:  

API:  
- please make sure you run it using Amber.Music.Api profile so that it can run on http port 5000, or https port 5001  
- in case you want/need to run the API on a different port, the UI will need the to have the API url updating in app.settings.ts  

UI:  
- the usual Angular process, from the root of the UI folder  
- ```npm install```  
- ```ng serve -o```  
  
### Functionality implemented  
- searching artists by name  
- artists search results pagination  
- retrieve the lyrics for all their songs (optimisation to run in parallel which is 2/3 faster than in a sequential run)  
- used the NuGet package to consume MusicBrainz library instead of re-writing the http requests, but an example of such a call can be observed when consuming the lyrics API
- compiled various values out of the lyrics data 
- modals to show the Lyrics for the song with the least and most number of words
- comparison of the latest 2 searched artists for which a table is shown where larger numbers are better and highlighted
- showing a graph for their releases over the years
- implemented loading spinners while data is retrieved or compiled
- caching of ran reports so that subsequent accesses can be faster
- clearing of the current data display
- tests for various parts of the application on the backend to serve as examples

### Improvements
- Angular code could be refactored into multiple components to tidy it up and make it easier to maintain  
- Better error handling both on the frontend and backend  
- Running and distributing the application via containers  
- Considering a storage/caching mechanism, rather than an in memory one  
- Convention based DI registrations  
- Mining more data to make the dashboard look more professional,
- etc.

  
