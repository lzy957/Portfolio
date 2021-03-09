#include "cfile.h"
#include <fstream>
#include <string.h>

CFile::CFile()
{
    E00=new CE00;
    search=new CSearch;

}

void CFile::FileOpen(const char* filename)
{
    FILE *fp;
    fp=fopen(filename,"r");
    if(!fp)
    {
        fputs("File cannot be opened.",stderr);
        exit(0);
    }

    char * line = NULL;
    string stempt;
    size_t len = 0;
    ssize_t read;

//    const char *d=" ";
//    char *p;
    bool isarc=false;
    bool istol=false;
    bool islog=false;
    bool isifo=false;
    bool isatt=false;
    double scalepre;
    int nodetablecount=0;
    int arcnumcount=0;
    int attrinumcount=0;
    int pointnumt=0;
    double xmax=0;double xmin=50000000000;double ymax=0;double ymin=50000000000;
    int tolcount=0;
    E00->initialnode(892);
    while ((read = getline(&line, &len, fp)) != -1)
    {
        stempt=line;
//        p = strtok(line,d);
        if(stempt.find("EXP")==0)
        {
            E00->EXP=new CEXP;
            char * tempt=new char[100];
            E00->EXP->iscompressed=(stempt[5]-'0');
            strcpy(tempt,stempt.substr(6,read-6).data());
            E00->EXP->path=tempt;
        }
        else if(stempt.find("ARC")==0)
        {
            E00->arcprecision=(stempt[5]-'0');
            if(E00->arcprecision==2)
                scalepre=1000000;
            else if(E00->arcprecision==3)
                scalepre=100000000;
            if(isifo==false&&isatt==false)
                isarc=true;
        }
        else if(isarc)
        {
            CArc *arctempt=new CArc;
            CNodetable * nodetempt=new CNodetable;
            char *tc=new char[30];
            int ti[7]={0};
            int icount=0;
            int count=0;
            for(int i=0;i<read-1;i++)
            {
                if(line[i]!=' ')
                {
                    tc[count]=line[i];
                    count++;
                    if(line[i+1]==' ')
                    {
                       ti[icount]=atoi(tc);
                       icount++;
                       count=0;
                       memset(tc,0,sizeof(tc));
                    }
                 }
            }
            ti[6]=atoi(tc);
            if(ti[0]==-1)
            {
                isarc=false;
                if(tc)
                    delete[] tc;
                continue;
            }
            arctempt->fnode=new CNode;
            arctempt->tnode=new CNode;
                arctempt->auto_num=ti[0];
                arctempt->user_num=ti[1];
                arctempt->fnode->num=ti[2];
                arctempt->tnode->num=ti[3];
                arctempt->lpoly=ti[4];
                arctempt->rpoly=ti[5];
                arctempt->nodecount=ti[6];
                double cort=0.0;
                double node[800]={0};
                icount=0;
                if(E00->arcprecision==2)
                {
                    for(int i=0;i<(arctempt->nodecount+1)/2;i++)
                    {
                        memset(tc,0,sizeof(tc));
                        read = getline(&line, &len, fp);
                        count=0;
                        for(int k=0;k<read-1;k++)
                        {
                            if(line[k]!=' ')
                            {
                                tc[count]=line[k];
                                count++;
                                if(line[k+1]==' ')
                                {
                                   node[icount]=double(stod(tc))*scalepre;
                                   icount++;
                                   count=0;
                                   memset(tc,0,sizeof(tc));
                                }
                             }
                    }
                        node[icount]=double(stod(tc))*scalepre;
                        icount++;
                }
                }
                    else if(E00->arcprecision==3)
                    {
                        for(int i=0;i<arctempt->nodecount;i++)
                        {
                            memset(tc,0,sizeof(tc));
                            read = getline(&line, &len, fp);
                            stempt=line;
                            count=0;
                            strcpy(tc,stempt.substr(2,21).data());
                            cort=double (stof(tc));
                            node[icount]=cort*scalepre;
                            icount++;
                            strcpy(tc,stempt.substr(23,42).data());
                            cort=double (stof(tc));
                            node[icount]=cort*scalepre;
                            icount++;
//                            for(int k=0;k<read-1;k++)
//                            {
//                                if(line[k]!=' ')
//                                {
//                                    tc[count]=line[k];
//                                    count++;
//                                    if(line[k+1]==' ')
//                                    {
//                                        double test;
//                                        test=double(stod(tc))*scalepre;
//                                       node[icount]=test;
//                                       icount++;
//                                       count=0;
//                                       memset(tc,0,sizeof(tc));
//                                    }
//                                 }
//                        }

                        }
                    }

                for(int j=0;j<ti[6]*2-1;)
                {
                    if(node[j]>xmax)
                        xmax=node[j];
                    else if(node[j]<xmin)
                        xmin=node[j];
                    if(node[j+1]>ymax)
                        ymax=node[j+1];
                    else if(node[j+1]<ymin)
                        ymin=node[j+1];
                    j=j+2;
                }
                arctempt->fnode->pt.setX(node[0]);
                arctempt->fnode->pt.setY(node[1]);
                arctempt->tnode->pt.setX(node[ti[6]*2-2]);
                arctempt->tnode->pt.setY(node[ti[6]*2-1]);
                for(int j=2;j<ti[6]*2-2;j=j+2)
                {
                    CPoint *p=new CPoint;
                    p->setX(node[j]);
                    p->setY(node[j+1]);
                    arctempt->minornodelist.push_back(p);
                }
                E00->arclist.push_back(arctempt);
                if(E00->nodetable.size()>0)
                    if(E00->nodetable.at(nodetablecount-1)->fnum==ti[2])
                {
                    arcnumcount++;
                    E00->nodetable.at(nodetablecount-1)->arcnum[arcnumcount]=ti[1];
                    E00->nodetable.at(nodetablecount-1)->arclist.push_back(arctempt);

                }
                else
                {
                    nodetempt->fnum=ti[2];
                    nodetempt->arcnum[0]=ti[1];
                    nodetempt->arclist.push_back(arctempt);
                    E00->nodetable.push_back(nodetempt);
                    nodetablecount++;
                    arcnumcount=0;
                }
                E00->pointarray.at(ti[2]-1)->setPoint(arctempt->fnode->pt);
                E00->pointarray.at(ti[3]-1)->setPoint(arctempt->tnode->pt);
                if(pointnumt<ti[2])
                {
                    pointnumt=ti[2];
//                    ptempt=arctempt->fnode;
//                    E00->pointarray.push_back(ptempt);
                }
                else if(pointnumt<ti[3])
                {
                    pointnumt=ti[3];
//                    ptempt=arctempt->tnode;
//                    E00->pointarray.push_back(ptempt);
                }

        }
        else if(stempt.find("TOL")==0)
        {
            E00->TOL=new CTOL;
            E00->TOL->precision=(stempt[5]-'0');
            istol=true;
        }
        else if(istol)
        {
            char *tc=new char[30];
            int ti[7]={0};
            int icount=0;
            int count=0;
            for(int i=0;i<read-1;i++)
            {
                if(line[i]!=' ')
                {
                    tc[count]=line[i];
                    count++;
                    if(line[i+1]==' ')
                    {
                       ti[icount]=double(stod(tc));
                       icount++;
                       count=0;
                       memset(tc,0,sizeof(tc));
                    }
                 }
            }
            ti[icount]=double(stod(tc));
            if(ti[0]==-1)
            {
                istol=false;
//                if(tc)
//                    delete[] tc;
                continue;
            }
            E00->TOL->tol[tolcount].num=ti[0];
            E00->TOL->tol[tolcount].isverified=ti[1];
            E00->TOL->tol[tolcount].tol=ti[2];
            tolcount++;
        }
        else if(stempt.find("SIN")==0)
        {
            E00->sin=(stempt[5]-'0');
            read=getline(&line, &len, fp);
        }
        else if(stempt.find("LOG")==0)
        {
            E00->sin=(stempt[5]-'0');
            islog=true;
        }
        else if(stempt.find("EOL")!=0&&islog)
        {
            CLOG *logtempt=new CLOG;
            char* tempt=new char[50];
            strcpy(tempt,stempt.substr(0,3).data());
            logtempt->year=atoi(tempt);
            strcpy(tempt,stempt.substr(4,5).data());
            logtempt->month=atoi(tempt);
            strcpy(tempt,stempt.substr(6,7).data());
            logtempt->day=atoi(tempt);
            strcpy(tempt,stempt.substr(8,9).data());
            logtempt->hours=atoi(tempt);
            strcpy(tempt,stempt.substr(10,11).data());
            logtempt->min=atoi(tempt);
            strcpy(tempt,stempt.substr(12,15).data());
            logtempt->connecttimem=atoi(tempt);
            strcpy(tempt,stempt.substr(16,21).data());
            logtempt->cputimes=atoi(tempt);
            strcpy(tempt,stempt.substr(22,27).data());
            logtempt->iotimes=atoi(tempt);
            strcpy(tempt,stempt.substr(28,read-1).data());
            logtempt->commandline=tempt;
            E00->loglist.push_back(logtempt);
            read=getline(&line, &len, fp);
//            if(tempt)
//                delete[] tempt;
        }
        else if(stempt.find("EOL")==0)
        {
            islog=false;
            continue;
        }
        else if(stempt.find("IFO")==0)
        {
            E00->cattri=new CAttribution;
            E00->cattri->infoprecision=(stempt[5]-'0');
            read = getline(&line, &len, fp);
            stempt=line;
            char* tempt=new char[40];
            strcpy(tempt,stempt.substr(0,31).data());
            E00->cattri->name=tempt;
            strcpy(tempt,stempt.substr(32,33).data());
            E00->cattri->externalflag=tempt;
            strcpy(tempt,stempt.substr(34,37).data());
            E00->cattri->attrinum=atoi(tempt);
            strcpy(tempt,stempt.substr(38,41).data());
            E00->cattri->attrinumtotal=atoi(tempt);
            strcpy(tempt,stempt.substr(42,45).data());
            E00->cattri->datalength=atoi(tempt);
            strcpy(tempt,stempt.substr(46,55).data());
            E00->cattri->recordnum=atoi(tempt);
//            E00->datamatrix=new double *[pointnumt];
//            for(int num=0;num<pointnumt;num++)
//                E00->datamatrix[num]=new double[pointnumt];
//            for(int i=0;i<pointnumt;i++)
//            {
//                for(int j=0;j<pointnumt;j++)
//                   E00->datamatrix[i][j]=0;
//            }

            isifo=true;
        }
        else if(isifo)
        {
            char* tempt=new char[40];
        for(int i=0;i<E00->cattri->attrinum-1;i++)
        {
            attristruct *attritempt=new attristruct;
            stempt=line;
            strcpy(tempt,stempt.substr(0,15).data());
            attritempt->name=tempt;
            strcpy(tempt,stempt.substr(16,18).data());
            attritempt->sizebina=atoi(tempt);
            strcpy(tempt,stempt.substr(21,24).data());
            attritempt->startpos=atoi(tempt);
            strcpy(tempt,stempt.substr(28,31).data());
            attritempt->ofwidth=atoi(tempt);
            strcpy(tempt,stempt.substr(32,33).data());
            attritempt->ofprecision=atoi(tempt);
            strcpy(tempt,stempt.substr(34,36).data());
            attritempt->type=atoi(tempt);
            strcpy(tempt,stempt.substr(49,64).data());
            attritempt->sfname=tempt;
            strcpy(tempt,stempt.substr(65,68).data());
            attritempt->attrindex=atoi(tempt);
            E00->cattri->attrilist.push_back(attritempt);
            read=getline(&line, &len, fp);
        }
        attristruct *attritempt=new attristruct;
        stempt=line;
        strcpy(tempt,stempt.substr(0,15).data());
        attritempt->name=tempt;
        strcpy(tempt,stempt.substr(16,18).data());
        attritempt->sizebina=atoi(tempt);
        strcpy(tempt,stempt.substr(21,24).data());
        attritempt->startpos=atoi(tempt);
        strcpy(tempt,stempt.substr(28,31).data());
        attritempt->ofwidth=atoi(tempt);
        strcpy(tempt,stempt.substr(32,33).data());
        attritempt->ofprecision=atoi(tempt);
        strcpy(tempt,stempt.substr(34,36).data());
        attritempt->type=atoi(tempt);
        strcpy(tempt,stempt.substr(49,64).data());
        attritempt->sfname=tempt;
        strcpy(tempt,stempt.substr(65,68).data());
        attritempt->attrindex=atoi(tempt);
        E00->cattri->attrilist.push_back(attritempt);
        isatt=true;
        isifo=false;
//        if (tempt)
//            delete[] tempt;
        }
        else if(isatt)
        {
//            char tcatt=new char(30);
            char tcatt[30]={""};
            CAttrecord* recordtempt=new CAttrecord;
            double tiatt[15]={0};
            int icount=0;
            int count=0;
            for(int i=0;i<read-1;i++)
            {
                if(line[i]!=' ')
                {
                    tcatt[count]=line[i];
                    count++;
                    if(line[i+1]==' ')
                    {
                       tiatt[icount]=double(stod(tcatt));
                       recordtempt->attrirecord.push_back(tiatt[icount]);
                       icount++;
                       count=0;
                       memset(tcatt,0,30);
                    }
                 }
            }
            read=getline(&line, &len, fp);
            count=0;
            for(int i=0;i<read-1;i++)
            {
                if(line[i]!=' ')
                {
                    tcatt[count]=line[i];
                    count++;
                    if(line[i+1]==' ')
                    {
                       tiatt[icount]=double(stod(tcatt));
                       recordtempt->attrirecord.push_back(tiatt[icount]);
                       icount++;
                       count=0;
                       memset(tcatt,0,30);
                    }
                 }
            }
            if(E00->cattri->attrinum>14)
            {
                read=getline(&line, &len, fp);
                count=0;
                for(int i=0;i<read-1;i++)
                {
                    if(line[i]!=' ')
                    {
                        tcatt[count]=line[i];
                        count++;
                        if(line[i+1]==' ')
                        {
                           tiatt[icount]=double(stod(tcatt));
                           recordtempt->attrirecord.push_back(tiatt[icount]);
                           icount++;
                           count=0;
                           memset(tcatt,0,30);
                        }
                     }
                }
                tiatt[icount]=strtod(tcatt,NULL);
                memset(tcatt,0,30);
            }
            recordtempt->attrirecord.push_back(tiatt[icount]);
//            recordtempt->fnodenum=ti[0];
//            recordtempt->tnodenum=ti[1];
//            recordtempt->lpolynum=ti[2];
//            recordtempt->rpolynum=ti[3];
//            recordtempt->length=ti[4];
//            recordtempt->autorivernum=ti[5];
//            recordtempt->riverid=ti[6];
//            recordtempt->type=ti[7];
//            *(*(E00->datamatrix+int(ti[0])-1)+int(ti[1]-1))=ti[14];
            int tempti=0,temptj=0;
            tempti=int(tiatt[0]-1);
            temptj=int(tiatt[1]-1);
            E00->dmatrix[tempti][temptj]=tiatt[4];
            E00->cattri->attrecordlist.push_back(recordtempt);
            attrinumcount++;
            if(attrinumcount==E00->cattri->recordnum)
                isatt=false;
//            if(tc)
//                delete[] tc;
        }
    }
    prj=new CProject;
    prj->scalex=xmax-xmin;
    prj->scaley=ymax-ymin;
    prj->xmax=xmax;
    prj->xmin=xmin;
    prj->ymin=ymin;
    prj->ymax=ymax;
    E00->pointnum=pointnumt;
//    if (line)
//        free(line);

}
