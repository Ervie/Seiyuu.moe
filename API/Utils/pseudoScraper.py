#!/usr/bin/python

import sqlite3
import urllib.request
import json

def CallApi (index, retryCount):
    try:
        if (retryCount < maxRetryCount):
            contents = urllib.request.urlopen("http://api.jikan.moe/person/" + str(index)).read()
            
            return json.loads(contents)
        else:
            return ""

    except urllib.error.HTTPError as error:
        print ('HTTP error occured for id = ' + str(i) + ', reason = ' + str(error.read()))
        retryCount += 1
        return CallApi(index, retryCount)
    except urllib.error.URLError as error:
        print ('HTTP error occured for id = ' + str(i) + ', reason = ' + str(error.read()))
        retryCount += 1
        return CallApi(index, retryCount)

startIndex = 35000
endIndex = 40000
maxRetryCount = 3

conn = sqlite3.connect('SeiyuuInterlinkDB.db')

print ("Opened database successfully");

conn.execute("SELECT * from Seiyuus;")
conn.commit()
cur = conn.cursor()   

for i in range(startIndex, endIndex):

    cur.execute('SELECT MalId FROM Seiyuus where MalId = ' + str(i))
    data = cur.fetchone()
    if (data != None):
        pass
    seiyuu = CallApi(i, 1)
    
    
    if seiyuu == "":
        print ("Exceeded retry count for id " + str(i))

    elif seiyuu["given_name"] == None:
        print ("Skipping " + str(seiyuu["name"]) + ", mal_id = " + str(seiyuu["mal_id"]) + ", reason: non Japanese")
        pass
    elif len(seiyuu["voice_acting_role"]) < 1:
        print ("Skipping " + str(seiyuu["name"]) + ", mal_id = " + str(seiyuu["mal_id"]) + ", reason: not a Seiyuu")
        pass
    else:
        conn.execute("INSERT INTO Seiyuus (Name, ImageUrl, MalId) VALUES (\'" + str(seiyuu["name"]) + "\', \'" + str(seiyuu["image_url"])  + "\', " + str(seiyuu["mal_id"]) + ");")
        print ("Inserted " + (seiyuu["name"]) + ", mal_id = " + str(seiyuu["mal_id"]))
        conn.commit()
    pass

conn.close()



