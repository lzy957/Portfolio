#include "cindex.h"
#include "iostream"
using namespace std;
CIndex::CIndex()
{

}

void utf8ToGb2312(std::string& strUtf8)
{
QTextCodec* utf8Codec= QTextCodec::codecForName("utf-8");
QTextCodec* gb2312Codec = QTextCodec::codecForName("gb2312");

QString strUnicode= utf8Codec->toUnicode(strUtf8.c_str());
QByteArray ByteGb2312= gb2312Codec->fromUnicode(strUnicode);

strUtf8= ByteGb2312.data();
}

void CIndex::NameIndexset(CCityList fulldataset)
{
    char Letters[13][2]={'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
    for(int i=0;i<13;i++)
    {
        CNameIndex* nameset=new CNameIndex;
        nameset->SetLetter(Letters[i]);
        nameset->NameClassify(fulldataset);
        this->Nameindex.push_back(nameset);
    }
}

CChncity* CIndex::SearchNameIndex(QString input, CCityList fulldataset, CProject *proj)
{

    string name=input.toStdString();
    CChncity* tempt=new CChncity;
    tempt->name=input.toStdString();
    utf8ToGb2312(name);

    int len = name.length();
    char* py=new char[len];
    name.copy(py,len,0);

    char* output=new char[20];
    bool isutf8=tempt->cv->is_utf8_string(py);
    if (isutf8)
    {
            tempt->cv->pinyin_utf8(py, output,true,false,true);
            tempt->cv->sFirstLetter=output;
     }
    else
        tempt->cv->FirstLetter(py);

    list<CNameIndex*>::iterator i;
    for(i=this->Nameindex.begin();i!=this->Nameindex.end();++i)
    {
        if((*i)->Letter[0]==tempt->cv->sFirstLetter[0]||(*i)->Letter[1]==tempt->cv->sFirstLetter[0])
        {
            if(isutf8)
                return (*i)->NameSearch(tempt->cv->sFirstLetter,isutf8,proj);
            else
            return (*i)->NameSearch(tempt->cv->sFirstLetter,isutf8,proj);
    }

}
}
