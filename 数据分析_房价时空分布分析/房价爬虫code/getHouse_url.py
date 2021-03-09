# -*- coding: utf-8 -*-
"""
Spyder Editor

This is a temporary script file.
"""
import urllib2  
import time    
from bs4 import BeautifulSoup  
time.clock()
list_href = []
f=open('jiangxia_url.txt','w')
for i in range(1,47):
    print(i)
    url = 'http://wh.lianjia.com/xiaoqu/hannan/pg'+str(i)+'/'  
    page = urllib2.urlopen(url)  
    soup = BeautifulSoup(page)  
    for link in soup.find_all('li',class_="clear xiaoquListItem"):
         context = link.get("data-housecode")
         list_href.append("https://wh.lianjia.com/xiaoqu/"+context+"/\n")
         print(context)
f.writelines(list_href)
f.close()
print(time.clock())  

