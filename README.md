#RegionalAdvertising
Minimal Api dotnet project for getting advertising by location which are readed from file!
the project updaloads a simple `key:value` structured file and Cach it!
#Futures

**File Reading**
Reads file format: `"location": "Ad1", "Ad2"`
**Caching**
Saves file to InMemoryCache for reading via method!
**Search by location**
gives advertisings by searching location
**Testing**
Using 'xunit' tests with 'FluentAssertion' and 'FakeItEasy'


#File Example

```txt
"/ru": "Крутая реклама", "Яндекс.Директ"
"/us": "Google Ads", "Super Bowl Promo"
```
##Methods
```
/readLocations - HttpPost method reads file from filepath! Filepath is required!
/ads - HttpGet method gives advertising by location! location is requred field!
```
