import urllib2
from bs4 import BeautifulSoup
import re
import csv

list7=[]
list8=[]
list0=[]
count=0
num=2630
csvfile = open('jiangxia_coord.csv', 'w')
writer = csv.writer(csvfile)
data = []    
with open('jiangxia_url.txt','rb') as f:
    for line in f:
            #print(line)
        url1=line.decode('utf-8')
        count=count+1
            #print(url)
        page1=urllib2.urlopen(url1)
        content=page1.read().decode('utf-8')
        coord_xiaoqu = re.findall(r'xiaoqu=".{1,100}?"',content)
        #print(coord_xiaoqu)
        if(coord_xiaoqu!=None):
            for i in coord_xiaoqu:
                #print(i[8:-2])
                #if(i[]=='/'):
                list7.append(i[9:-2])
                #print (list7[0])
        else:
            list7.append(0)
        
        coord_menmian=re.findall(r'mendian=".{1,100}?"',content)
        if(coord_menmian!=None):
        #print(coord_menmian)
            for i in coord_menmian:
                #print(i[10:-1])
                #if(i[15]=='/'):
                list8.append(i[10:-1]+str('\n'))
                #print (list8[0])
            #print(count)
        else:
            list8.append(0)
        num=num+1
        list0.append(num)
        try:
            data.append((list0[num-2631],list7[count-1], list8[count-1]))
            #print(list0[count-1])
        except:
            count=count-1
            print(num)
            continue
            
writer.writerows(data)
csvfile.close()

    

