#include "ccitylist.h"

CCityList::CCityList()
{

}

CCityList::~CCityList()
{
//    for( this->cityi=this->CityList.begin();this->cityi!=this->CityList.end();++this->cityi)
//        delete(*this->cityi);
}

void CCityList::Display()
{
    for( this->cityi=this->CityList.begin();this->cityi!=this->CityList.end();++this->cityi)
        (*this->cityi)->Display();
}

void CCityList::GetFirstLetters()
{
    for( this->cityi=this->CityList.begin();this->cityi!=this->CityList.end();++this->cityi)
        (*this->cityi)->cv->FirstLetter((*this->cityi)->name);
}

void CCityList::Draw(CProject *proj)
{
    for( this->cityi=this->CityList.begin();this->cityi!=this->CityList.end();++this->cityi)
        (*this->cityi)->Draw(proj);
}
