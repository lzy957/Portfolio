import urllib2
from bs4 import BeautifulSoup
import re
import csv
import sys
reload(sys);
sys.setdefaultencoding("utf8")

def getKeyWords():
    with open('test_content.txt','rb') as f:
        for line in f:
            wd=line.decode('gbk').split('/')
            #print(wd[1])
            return wd
##                  request = urllib.request.Request(url)
##                  page = urllib.request.urlopen(request)
##                  data = page.read().decode('utf-8')
##                  soup = BeautifulSoup(data,'html.parser')
##                  tags = soup.select('dd a')
##                  res = [wd[:-1]+'|'+t['href']+'|'+t.get_text()+'\n' for t in tags]
##                  print (len(res))
##                  urld,wdd=line.decode('gbk').split('|')



def getCoord(page):
    content=page.read().decode('utf-8')
    coord = re.findall(r'coord=".{1,100}?">',content)
    print(coord)
    for i in coord:
        print(i[7:-2])
            #if(i[15]=='/'):
        list1.append(i[7:-2]+str('/n'))
        print (list1[0])
        count=count+1

def getCor():
    list7=[]
    with open('jiangxia_url.txt','rb') as f:
        for line in f:
            #print(line)
            url1=line.decode('gbk')
            #print(url)
            page1=urllib.request.urlopen(url1)
            content=page1.read().decode('utf-8')
            coord = re.findall(r'xiaoqu=".{1,100}?"',content)
    print(coord)
    for i in coord:
        print(i[8:-1])
            #if(i[15]=='/'):
        list1.append(i[8:-1]+str('/n'))
        print (list7[0])
        #count=count+1


if __name__ == '__main__':
    list0=[]
    list1=[]
    list2=[]
    list3=[]
    list4=[]
    list5=[]
    list6=[]
    data=[]
    csvfile = open('jiangxia.csv', 'w')
    writer = csv.writer(csvfile)
    count=0
    count1=2630
    #getCor()
    with open('jiangxia_url.txt','rb') as f:
        for line in f:
            #print(line)
            url=line.decode('gbk')
            #print(url)
            page=urllib2.urlopen(url)
            soup = BeautifulSoup(page)
            for link in soup.find_all('h1','detailTitle'):
                name = link.get_text()
                if(name!=None):
                    list0.append(name)
                else:
                    list0.append(0)
            count=count+1
            count1=count1+1
            list6.append(count1)
            print(count)
            address_tag = soup.find('div', attrs={'class': 'detailDesc'})
            if(address_tag!=None):
                address = address_tag.string
                list1.append(address)
            else:
                list1.append(0)
            #print(address)
            house_price_tag = soup.find('span', attrs={'class': 'xiaoquUnitPrice'})
            if(house_price_tag!=None):
                house_price = house_price_tag.string
                list2.append(house_price)
            else:
                list2.append(0)
            #print(house_price)
            info_tag = soup.find_all('span', attrs={'class': 'xiaoquInfoContent'})
            if(info_tag!=None and len(info_tag)>6):
                estste_price = info_tag[2].string
                list3.append(estste_price)
                building_count = info_tag[5].string
                list4.append(building_count)
                house_count = info_tag[6].string
                list5.append(house_count)
            else:
                list3.append(0)
                list4.append(0)
                list5.append(0)
            #print(list6[count-1])
            #print(list0[count-1])
            #print(list2[count-1])
            #print(list1[count-1])
            #print(list3[count-1])
            #print(list4[count-1])
            #print(list5[count-1])
            try:
                data.append((list6[count1-2631],list0[count-1], list2[count-1], list1[count-1], list3[count-1], list4[count-1],list5[count-1]))
            except:
                count=count-1
                print(count1)
                continue
  #  data = []
#for j in range(0,count-1):
 #   print(j)
  #  data.append((list6[j],list0[j], list2[j], list1[j], list3[j], list4[j],list5[j]))
writer.writerows(data)
csvfile.close()



