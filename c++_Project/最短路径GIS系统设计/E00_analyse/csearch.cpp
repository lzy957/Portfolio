#include "csearch.h"

CSearch::CSearch()
{
    acc=0;bcc=0;length=10000000;countg=0;count=0;
}

CSearch::~CSearch()
{
    acc=0;bcc=0;length=10000000;countg=0;
    memset(path,0,100);
    memset(path_tempt,0,100);
    memset(length_temp,0,100);
}

bool CSearch::findtonode(int numf,int numt, CE00 *ce00)
{
    bool isfrom=false;
    int *fnumlist=new int[20];
    int count=0;
    list<CArc*>::iterator i;
    for(i=ce00->arclist.begin();i!=ce00->arclist.end();++i)
    {
        if(numf==(*i)->fnode->num)
        {
            isfrom=true;
            fnumlist[count]=(*i)->tnode->num;
            count++;
            if((*i)->tnode->num==numt)
                return true;
        }
    }
    if(isfrom)
    {
        for(int j=0;j<count;j++)
            findtonode(fnumlist[j],numt,ce00);
    }
    else
        return false;

}



bool CSearch::shortpath(int k,CE00 *ce00)
{
     int i=0;int j=0;
     for(j=0;j<endnum;j++)
     {
         if(ce00->dmatrix[k-1][j]!=0)
         {
             if(j==endnum-1)
             {
                 acc++;
                 length_temp[acc]=length_temp[acc-1]+ce00->dmatrix[k-1][j];
                 path_tempt[++bcc]=j+1;
                 if(length_temp[acc]<length)
                 {
                     length=length_temp[acc];
                     countg=0;
                     for(i=0;i<bcc;i++)
                     {
                         path[i]=path_tempt[i];
                         countg++;
                     }
                 }
                 length_temp[acc--]=0;
                 length_temp[acc--]=0;
                 path_tempt[bcc--]=0;
                 path_tempt[bcc--]=0;
             }
             else
             {
                 acc++;
                 length_temp[acc]=length_temp[acc-1]+ce00->dmatrix[k-1][j];
                 path_tempt[++bcc]=j+1;
                 shortpath(j+1,ce00);
             }
         }
     else if(ce00->dmatrix[k-1][j]==0&&j==endnum-1)
         {
             length_temp[acc--]=0;
             path_tempt[bcc--]=0;
             break;
         }
     }
}

void CSearch::clear()
{
    acc=0;bcc=0;length=10000000;countg=0;
    memset(path,0,100);
    memset(path_tempt,0,100);
    memset(length_temp,0,100);
    pathlist.clear();
}

void CSearch::Draw(CProject *prj)
{
    list<CArc*>::iterator i;
    for( i=this->pathlist.begin();i!=this->pathlist.end();++i)
        (*i)->Drawpath(prj);

}

void CSearch::SetList(CE00 *e00)
{
    list<CArc*>::iterator i;
        for(int j=0;j<countg;j++)
        {
            for(i=e00->arclist.begin();i!=e00->arclist.end();++i)
            {
                if(path[j]==(*i)->fnode->num&&(*i)->tnode->num==path[j+1])
                {
                    pathlist.push_back(*i);
                }
            }
        }
}
