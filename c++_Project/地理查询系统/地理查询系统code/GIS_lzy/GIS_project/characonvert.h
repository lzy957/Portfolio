#ifndef CHARACONVERT_H
#define CHARACONVERT_H

#include <string>
#include <stdio.h>

////////////////////////////////////////////
/// 汉字转拼音函数 适合gb2312和utf8
///////////////////////////////////////////

#define HZ2PY_OUTPUT_BUF_ARRAY_SIZE 5120    //一个字段的拼音最长长度
#define HZ2PY_MAX_PINYIN_SIZE 30            //一个字的拼音最长长度
#define HZ2PY_UTF8_CHECK_LENGTH 20          //检查是否为utf8编码时所检查的字符长度

using namespace std;

//static bool In(wchar_t start,wchar_t end,wchar_t code);

class Characonvert
{
public:
    Characonvert();
     ~Characonvert();
    string sFirstLetter;
    char convert(wchar_t n);
    void FirstLetter(string name);

    bool safeAddToOutbuf(char* outbuf,int &iOutbuf,char* pinyinValue,int iPinyinValue);
    void pinyin_utf8(char* inbuf,char* outbuf, bool m_blnSimaple=false,bool polyphone_support=false,bool m_blnFirstBig=false,bool m_blnAllBiG=false,bool m_LetterEnd=false, bool m_unknowSkip=true,bool m_filterPunc=true);
    int is_utf8_string(char* utf);

};


#endif // CHARACONVERT_H
