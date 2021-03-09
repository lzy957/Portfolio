#ifndef CNAMEINDEX_H
#define CNAMEINDEX_H
#include "ccitylist.h"

class CNameIndex
{
public:
    CNameIndex();
    ~CNameIndex();

//    CCityList* Citiescell;
    list<CChncity*> Citiescell;
    list<CChncity*>::iterator i;

    char Letter[2];
    void SetLetter(char linput[]);
    void NameClassify(CCityList fulldataset);
    CChncity* NameSearch(string pinyin,bool is_utf8,CProject* proj);
};

#endif // CNAMEINDEX_H
