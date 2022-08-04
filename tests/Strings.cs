namespace tests;

public static class Strings
{
    public static readonly string DrSeuss =
@"
I AM SAM. I AM SAM. SAM I AM.

THAT SAM-I-AM! THAT SAM-I-AM! I DO NOT LIKE THAT SAM-I-AM!

DO WOULD YOU LIKE GREEN EGGS AND HAM?

I DO NOT LIKE THEM,SAM-I-AM.
I DO NOT LIKE GREEN EGGS AND HAM.
".Trim();

    public static readonly string DrSeussLzssEncoded =
@"
I AM SAM. <10,10>SAM I AM.

THAT SAM-I-AM! <15,15>I DO NOT LIKE<29,15>

DO WOULD YOU LIKE GREEN EGGS AND HAM?

<69,16>EM,<69,8>.
<29,14><64,18>.
".Trim();    

}
