#include "cnameindex.h"

CNameIndex::CNameIndex()
{

}

CNameIndex::~CNameIndex()
{

}

void CNameIndex::SetLetter(char linput[])
{
    this->Letter[0]=linput[0];
    this->Letter[1]=linput[1];
}

void CNameIndex::NameClassify(CCityList fulldataset)
{
//    list<CChncity*>::iterator i;
    fulldataset.GetFirstLetters();
    for(this->i=fulldataset.CityList.begin();this->i!=fulldataset.CityList.end();++this->i)
    {
        if((*i)->cv->sFirstLetter.find(this->Letter[0])==0
                ||(*i)->cv->sFirstLetter.find(this->Letter[1])==0)
            this->Citiescell.push_back(*i);

    }
}

CChncity* CNameIndex::NameSearch(string pinyin, bool is_utf8, CProject *proj)
{
//    CChncity* tempt=new CChncity();
    for(this->i=this->Citiescell.begin();this->i!=this->Citiescell.end();++(this->i))
    {
        if(is_utf8)
        {
            if((*(this->i))->cv->sFirstLetter.find(pinyin)==0)
                return *i;
//                (*this->i)->Mark(proj);
//                (*this->i)->Display();

        }
    else
        {
        if((*(this->i))->cv->sFirstLetter.find(pinyin)==0)
            return *i;
//            (*this->i)->Mark(proj);
//            (*this->i)->Display();

        }
    }
}
