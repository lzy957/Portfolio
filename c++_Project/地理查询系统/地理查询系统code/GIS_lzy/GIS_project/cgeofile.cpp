#include "cgeofile.h"
#include <fstream>
#include <string.h>
#include "stdio.h"

using namespace std;

CGeoFile::CGeoFile()
{
    this->Symbolset=new CListSymbol;
    this->map=new CGeomap;
//    this->Cityset=new CCityList;
}

CGeoFile::~CGeoFile()
{
    list<CChncity*>::iterator i;
    for(i=this->Cityset.CityList.begin();i!=this->Cityset.CityList.end();++i)
        delete(*i);
}

void CGeoFile::GraphicFileOpen(const char* filename)
 {
     FILE *fp;
     fp=fopen(filename,"r");
     if(!fp)
     {
         fputs("File cannot be opened.",stderr);
         exit(0);
     }
     int l,b,r,t;
     fscanf(fp,"%d,%d",&l,&t);
     map->wrect.left=l;
     map->wrect.top=t;
     fscanf(fp,"%d,%d",&r,&b);
     map->wrect.right=r;
     map->wrect.bottom=b;
     map->proj=new CProject;
     map->proj->scaley=t-b;
     map->proj->scalex=r-l;
     map->scaley=t-b;
     map->scalex=r-l;
     while(!feof(fp))
    {
//        CGeomap* tempmap=new CGeomap;
        int n=0;
        fscanf(fp,"%d",&n);
        this->map->layernum=n;
        for(int i=0;i<n;i++)
        {
            CGeolayer* templayer=new CGeolayer;
            fscanf(fp,"%d",&templayer->namesize);
            char a[20]={0};
//            fscanf(fp,"%s",&(templayer->layername));
            fscanf(fp,"%s",a);
            templayer->layername=a;
//            cout<<templayer->layername;
            fscanf(fp,"%d",&templayer->objectnum);
            for(int k=0;k<templayer->objectnum;k++)
            {
                int type,x,y;
                fscanf(fp,"%d",&type);
                switch(type)
                {
                case 0:
                {
                    CGeometry* tempobject=new CGeopoint;
                    tempobject->type=0;
                    do
                    {
                        fscanf(fp,"%d,%d",&x,&y);
                        ((CGeopoint*) tempobject)->x=x;
                        ((CGeopoint*) tempobject)->y=y;
                    }
                    while(x!=-99999);
                    templayer->Geoobjects.push_back(tempobject);
                    break;
                }
                case 1:
                {
                    CGeometry* tempobject=new CGeopolyline;
                    tempobject->type=1;
                    fscanf(fp,"%d,%d",&x,&y);
                    while(x!=-99999)
                    {
                        CGeopoint* pt=new CGeopoint;
                        pt->x=x;
                        pt->y=y;
                        ((CGeopolyline*) tempobject)->Polyline.push_back(pt);
                        fscanf(fp,"%d,%d",&x,&y);
                      }
                    templayer->Geoobjects.push_back(tempobject);
                    break;
                }
                  case 2:
                {
                    CGeometry* tempobject=new CGeopolygon;
                    tempobject->type=2;
                    fscanf(fp,"%d,%d",&x,&y);
                    while(x!=-99999)
                    {
                        CGeopoint *pt=new CGeopoint;
                        pt->x=x;
                        pt->y=y;
                        ((CGeopolygon*) tempobject)->Polygon.push_back(pt);
                        fscanf(fp,"%d,%d",&x,&y);
                    }
                    templayer->Geoobjects.push_back(tempobject);
                    break;
                }
                }
            }
            this->map->Geolayers.push_back(templayer);
//            fscanf(fp,"%s",a);
        }
//        this->maps.push_back(tempmap);
    }
    fclose(fp);
 }

void CGeoFile::SybFileOpen(const char *filename)
{
    FILE *fp;
    fp=fopen(filename,"r");
    if(!fp)
    {
        fputs("File cannot be opened.",stderr);
        exit(0);
    }
    while(!feof(fp))
    {
        int ln=0;
        fscanf(fp,"%d",&ln);
        this->Symbolset->n=ln;
        for(int i=0;i<ln;i++)
        {
            CSymbol* tempsymbol=new CSymbol;
            char a[20]={0};
            fscanf(fp,"%s",a);
            tempsymbol->layernames=a;
            fscanf(fp,"%d",&tempsymbol->isline);
             if(tempsymbol->isline==1)
            {
                 fscanf(fp,"%lf",&tempsymbol->linewidth);
                 fscanf(fp,"%d,%d,%d",&tempsymbol->lR,&tempsymbol->lG,&tempsymbol->lB);
            }
            fscanf(fp,"%d",&tempsymbol->isfilled);
            if(tempsymbol->isfilled==1)
            {
                fscanf(fp,"%d,%d,%d",&tempsymbol->fR,&tempsymbol->fG,&tempsymbol->fB);
            }
            this->Symbolset->Symbollist.push_back(tempsymbol);
        }
    }
    /*
    list<CGeomap*>::iterator mapn;
    list<CGeolayer*>::iterator layern;
    while(!feof(fp))
    {
        int n=0;
        fscanf(fp,"%d",&n);
        for(int i=0;i<n;i++)
        {
            mapn=this->maps.begin();
            char a[20]={0};
            string stemp;
            fscanf(fp,"%s",a);
            stemp=a;
            layern=(*mapn)->Geolayers.begin();
            while((*layern)->layername!=stemp)
            {
                int t=((*layern)->layername==stemp);
                ++layern;
            }
//                fscanf(fp,"%d",& (*layern)->isline);
//                fscanf(fp,"%f",&(*layern)->linewidth);
//                fscanf(fp,"%d,%d,%d",&(*layern)->lR,&(*layern)->lG,&(*layern)->lB);
//                fscanf(fp,"%d",& (*layern)->isfilled);
//                fscanf(fp,"%d,%d,%d",&(*layern)->fR,&(*layern)->fG,&(*layern)->fB);
        }
    }*/
    fclose(fp);
}

void CGeoFile::CityFileOpen(const char *filename)
{
    FILE *fp;
    size_t result;
    fp=fopen(filename,"r");
    if(!fp)
    {
        fputs("File cannot be opened.",stderr);
        exit(0);
    }
    //获取文件大小
    fseek(fp,0,SEEK_END);
    long ISize=ftell(fp);
    rewind(fp);

    //分配内存空间
    char* buffer=(char*)malloc(sizeof(char)*ISize);
    if(buffer==NULL)
    {
        fputs("Memory error.",stderr);
        exit(1);
    }

    //将文件拷贝到buffer中
    result=fread(buffer,sizeof(char),ISize,fp);
    if(result!=ISize)
    {
        fputs("Reading error.",stderr);
        exit(2);
    }
    fclose(fp);

    // 按数据结构存储数据
    CChncity* tempcity=new CChncity;
    tempcity->name=strtok(buffer," ");
    tempcity->x=atoi(strtok(NULL," "));
    tempcity->y=atoi(strtok(NULL,"\n"));
    this->Cityset.CityList.push_back(tempcity);

    char* word;
    while ((word=strtok(NULL," "))!=NULL)
    {
        CChncity* tempcity=new CChncity;
        tempcity->name=word;
        tempcity->x=atoi(strtok(NULL," "));
        tempcity->y=atoi(strtok(NULL,"\n"));
        this->Cityset.CityList.push_back(tempcity);
    }
}

void CGeoFile::MatchSymbol(CListSymbol *Symbolset)
{
    list<CSymbol*>::iterator i;
    list<CGeolayer*>::iterator j;
    for(i=Symbolset->Symbollist.begin();i!=Symbolset->Symbollist.end();++i)
    {
        for(j=this->map->Geolayers.begin();j!=this->map->Geolayers.end();++j)
        {
            if((*i)->layernames.find((*j)->layername)==0)
            {
                CSymbol* tempt=new CSymbol;
                tempt=*i;
                (*j)->symbol=tempt;
            }
        }
    }
}

void CGeoFile::FileOpen()
{
    this->GraphicFileOpen("//Users//apple//Downloads//面向对象的 GIS//practise_3//data//china.dat");
    this->SybFileOpen("//Users//apple//Downloads//面向对象的 GIS//practise_3//data//china.opt");
    this->CityFileOpen("//Users//apple//Downloads//面向对象的 GIS//practise_3//data//chnCity.txt");
    this->MatchSymbol(Symbolset);
}
