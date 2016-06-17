#r "Microsoft.WindowsAzure.Storage"
#r "System.Runtime"
#r "System.Threading.Tasks"
#r "System.IO"

using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Collections.Generic;

public static async Task Run(ICloudBlob photoBlob, TraceWriter log, IAsyncCollector<Coupon> outputRecordQueueItem)
{
    // Expand this as we work, or possibly store this information in a MobileServiceTable
    // Store names are proper nouns and registered Trademarks. We should expect to find
    // them treated as such on the coupon. 
    // Corner case is if coupon uses ALL CAPS. 
    // Caps lock is cruise control for cool.
    string[] KnownStoreNames =
    {
        "JCPenney",
        "Quiznos",
        "Target"
    };

    // Ordered from most specific to least specific
    // We hope to match a word like "expiration date" before trying to match a generic
    // word like "ends"
    string[] ExpirationPhrase =
    {
        "expiration date",
        "expires on",
        "expiry date",
        "expires",
        "expiration",
        "expiry",
        "expire",
        "ends"
    };

    // The RecordId is the directory name.
    CloudBlobDirectory directory = photoBlob.Parent;
    string recordId = directory.Prefix.TrimEnd('/');
    
    log.Info($"recordId: {recordId}");

    log.Info($"Photo Uri: {photoBlob.Uri}");
    var visionClient = new VisionServiceClient("YOURCLIENTIDHERE");
    OcrResults result = null;
    try
    {
        result = await visionClient.RecognizeTextAsync(photoBlob.Uri.ToString(), "en");    
    }
    catch (ClientException ex)
    {
        log.Info(ex.Message);
    }

    // Store a dictionary of lines and their height (aka font size) on the coupon
    Dictionary<string, int> linesAndHeight = new Dictionary<string, int>();
    List<string> wordsFound = new List<string>();
    if (result != null)
    {
        foreach (var region in result.Regions)
        {
            foreach (var line in region.Lines)
            {
                string[] bboxAsString = line.BoundingBox.Split(',');
                int height = Convert.ToInt32(bboxAsString[3]);

                string sentence = "";

                foreach (var word in line.Words)
                {
                    if (word != null)
                    {
                        // log.Info($"Word found: {word.Text}");
                        wordsFound.Add(word.Text);
                        sentence = sentence + " " + word.Text;
                    }
                }

                // log.Info($"Line found: {sentence}, height: {height})");
                linesAndHeight[sentence] = height;
            }
        }
    }

    if (wordsFound.Count == 0)
    {
        log.Info($"No words found in image: {photoBlob.Uri}");
        outputRecordQueueItem = null;
        return;
    }

    string allWords = string.Join(" ", wordsFound);
    // log.Info($"All words found: {allWords}");

    //photoBlob.Metadata["ocr"] = output;
    //await photoBlob.SetMetadataAsync();

    // Look for an expiration phrase
    bool expiryFound = false;
    bool storeNameFound = false;
    string expirationDate = "";
    string storeName = "";
    foreach (string expiryPhrase in ExpirationPhrase)
    {
        if (!expiryFound)
        {
            for (int i = 0; i < wordsFound.Count; i++)
            {
                if (!expiryFound)
                {
                    string candidate = wordsFound[i];
                    if (string.Equals(expiryPhrase, candidate, StringComparison.OrdinalIgnoreCase))
                    {
                        // Assumes that the expiration date is the next word
                        expiryFound = true;
                        expirationDate = wordsFound[i + 1];
                    }
                }

                if (!storeNameFound)
                {
                    foreach (string knownName in KnownStoreNames)
                    {
                        string candidate = wordsFound[i];
                        if (IsAllUpper(candidate))
                        {
                            if (string.Equals(knownName, candidate, StringComparison.OrdinalIgnoreCase))
                            {
                                storeNameFound = true;
                                storeName = candidate;
                            }
                        }
                        else
                        {
                            if (string.Equals(knownName, candidate, StringComparison.Ordinal))
                            {
                                storeNameFound = true;
                                storeName = candidate;
                            }
                        }
                    }
                } // end if storeNameFound
            } // end for
        }
    }
    
    // Convert string expiry date to DateTime object
    DateTime expirationDateTime = DateTime.Now;
    if (expiryFound)
    {
        log.Info($"Expiration Date: {expirationDate}");
        expirationDateTime = DateTimeParser(expirationDate);
    }

    if (storeNameFound)
    {
        log.Info($"Store Name: {storeName}");
    }
    
    // Send Expiry and Description to Mobile Table
    var coupon = new Coupon
    {
        Id = recordId,
        Description = storeName,
        Expiry = expirationDateTime
    };
    await outputRecordQueueItem.AddAsync(coupon);
    log.Info($"Enqueue item: {coupon.Id}");
    log.Info($"C# Blob trigger function processed: {photoBlob.Uri}");
}

public static bool IsAllUpper(string word)
{
    for (int i = 0; i < word.Length; i++)
    {
        if (Char.IsLetter(word[i]) && !Char.IsUpper(word[i]))
        {
            return false;
        }
    }

    return true;
}

public class Coupon
{
    public string Id { get; set; }
    public string Description { get; set; }
    public DateTime Expiry { get; set; }
}

public static DateTime DateTimeParser(string dateString)
{
    // Remove white space
    char[] chars = dateString.ToCharArray();
    string dateStringNoWhiteSpace = "";
    for (int i = 0; i < chars.Length; i++)
    {
        if (!Char.IsWhiteSpace(chars[i]))
        {
            dateStringNoWhiteSpace = dateStringNoWhiteSpace + chars[i];
        }
    }

    string[] segments = dateStringNoWhiteSpace.Split('/');
    int month, day, year = -1;

    if (segments.Length == 1)
    {
        month = int.Parse(segments[0]);
        day = 1;
        year = 2016;
    }
    else if (segments.Length == 2)
    {
        month = int.Parse(segments[0]);
        day = int.Parse(segments[1]);
        year = 2016;
    }
    else if (segments.Length == 3)
    {
        month = int.Parse(segments[0]);
        day = int.Parse(segments[1]);
        year = int.Parse(segments[2]);
    }
    else
    {
        return DateTime.Now;
    }
    
    if (year < 100)
    {
        year = 2000 + year;
    }
    return new DateTime(year: year, month: month, day: day);
}
