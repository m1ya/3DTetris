//
//  NADUnityGlobal.m
//  Unity-iPhone
//

#import "NADUnityGlobal.h"

NSString* NADUnityCreateNSString(const char* string)
{
    if (string) {
        return @(string);
    } else {
        return @"";
    }
}

char* NADUnityMakeStringCopy(NSString* string)
{
    if (!string || 0 == string.length) {
        return NULL;
    }
    
    const char* utf8String = [string UTF8String];
    if (NULL == utf8String) {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(utf8String) + 1);
    strcpy(res, utf8String);
    return res;
}