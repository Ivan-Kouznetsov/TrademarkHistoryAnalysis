# Why?
The USPTO publishes historical trademark data on their website: [https://developer.uspto.gov/product/trademark-annual-xml-applications#product-files](https://developer.uspto.gov/product/trademark-annual-xml-applications#product-files)
This data is made available as XML, with numerous fields some of which are inapplicable or of low importance such as the date the file was compiled. If you want to analyse this data you will first need to extract salient information and to arrange it in a matter conducive to analysis. That is what this application does.

# What?
This application takes in a directory of one or more zipped XML files containing annual trademark data from the USPTO and converts the data contained therein into a SQLite database which you can then query using SQL by using the SQLite command line tool or 3rd party GUI tools.

The resulting tables have the following fields:

CaseFiles Table:

 - CaseFileId
 - FilingDate
 - SerialNumber
 - RegistrationDate
 - RegistrationNumber
 - Owner
 - OwnerTypeId
 - State
 - Country
 - Attorney
 - StatusCode
 - MarkLiteralElements
 - GoodsAndServices

	
CaseFileClass Table:

 - CaseFileClassId
 - CaseFileId
 - ClassId

# How?

Usage: TrademarkHistoryAnalysis [filename or directory] [name of sqlite db that porgram will create] 

## Technical Requirements:
In order to process the data efficiently and not be delayed by I/O, the XML data is processed in memory, which means you will want to run this program on a modern computer with a 64-bit processor and operating system and more than 5 GB of RAM. Because of this streamlined approach, the application is CPU bound, meaning that the faster the processor is the faster this program will run, as a consequence it will also use a lot of your processor's capacity while it runs.

On a modern PC it may process all currently available historical trademark data from the USPTO in less than half an hour.

# Original XML Data
The original data looks like this. It gets converted into tables as described above.

    <case-file>
              <serial-number>70011210</serial-number>
              <registration-number>0011210</registration-number>
              <transaction-date>20180707</transaction-date>
              <case-file-header>
                <filing-date>18840407</filing-date>
                <registration-date>18840527</registration-date>
                <status-code>800</status-code>
                <status-date>20131214</status-date>
                <mark-identification>SAMSON</mark-identification>
                <mark-drawing-code>3T19</mark-drawing-code>
                <republished-12c-date>19480817</republished-12c-date>
                <attorney-docket-number>SRT-0017</attorney-docket-number>
                <attorney-name>DENISE I. MROZ</attorney-name>
                <principal-register-amended-in>F</principal-register-amended-in>
                <supplemental-register-amended-in>F</supplemental-register-amended-in>
                <trademark-in>T</trademark-in>
                <collective-trademark-in>F</collective-trademark-in>
                <service-mark-in>F</service-mark-in>
                <collective-service-mark-in>F</collective-service-mark-in>
                <collective-membership-mark-in>F</collective-membership-mark-in>
                <certification-mark-in>F</certification-mark-in>
                <cancellation-pending-in>F</cancellation-pending-in>
                <published-concurrent-in>F</published-concurrent-in>
                <concurrent-use-in>F</concurrent-use-in>
                <concurrent-use-proceeding-in>F</concurrent-use-proceeding-in>
                <interference-pending-in>F</interference-pending-in>
                <opposition-pending-in>F</opposition-pending-in>
                <section-12c-in>T</section-12c-in>
                <section-2f-in>F</section-2f-in>
                <section-2f-in-part-in>F</section-2f-in-part-in>
                <renewal-filed-in>F</renewal-filed-in>
                <section-8-filed-in>F</section-8-filed-in>
                <section-8-partial-accept-in>F</section-8-partial-accept-in>
                <section-8-accepted-in>T</section-8-accepted-in>
                <section-15-acknowledged-in>T</section-15-acknowledged-in>
                <section-15-filed-in>F</section-15-filed-in>
                <supplemental-register-in>F</supplemental-register-in>
                <foreign-priority-in>F</foreign-priority-in>
                <change-registration-in>T</change-registration-in>
                <intent-to-use-in>F</intent-to-use-in>
                <intent-to-use-current-in>F</intent-to-use-current-in>
                <filed-as-use-application-in>T</filed-as-use-application-in>
                <amended-to-use-application-in>F</amended-to-use-application-in>
                <use-application-currently-in>T</use-application-currently-in>
                <amended-to-itu-application-in>F</amended-to-itu-application-in>
                <filing-basis-filed-as-44d-in>F</filing-basis-filed-as-44d-in>
                <amended-to-44d-application-in>F</amended-to-44d-application-in>
                <filing-basis-current-44d-in>F</filing-basis-current-44d-in>
                <filing-basis-filed-as-44e-in>F</filing-basis-filed-as-44e-in>
                <filing-basis-current-44e-in>F</filing-basis-current-44e-in>
                <amended-to-44e-application-in>F</amended-to-44e-application-in>
                <without-basis-currently-in>F</without-basis-currently-in>
                <filing-current-no-basis-in>F</filing-current-no-basis-in>
                <color-drawing-filed-in>F</color-drawing-filed-in>
                <color-drawing-current-in>F</color-drawing-current-in>
                <drawing-3d-filed-in>F</drawing-3d-filed-in>
                <drawing-3d-current-in>F</drawing-3d-current-in>
                <standard-characters-claimed-in>F</standard-characters-claimed-in>
                <filing-basis-filed-as-66a-in>F</filing-basis-filed-as-66a-in>
                <filing-basis-current-66a-in>F</filing-basis-current-66a-in>
                <renewal-date>20140527</renewal-date>
                <current-location>GENERIC WEB UPDATE</current-location>
                <location-date>20131214</location-date>
              </case-file-header>
              <case-file-statements>
                <case-file-statement>
                  <type-code>A00001</type-code>
                  <text>In the statement, Column 2, line 1 through 4, "such facsimile represents pictorially the scriptural character SAMSON in the act of slaying a lion" is deleted, and "The mark consists of a picture representing Samson and the lion on top of the stylized, partially-underlined word SAMSON." is inserted. The drawing is amended to appear as follows: PUBLISH NEW CUT</text>
                </case-file-statement>
                <case-file-statement>
                  <type-code>DM0000</type-code>
                  <text>The mark consists of a picture representing Samson and the lion on top of the stylized, partially-underlined word SAMSON.</text>
                </case-file-statement>
                <case-file-statement>
                  <type-code>GS0071</type-code>
                  <text>CORDS, LINES, [ TWINES, ] AND ROPES</text>
                </case-file-statement>
              </case-file-statements>
              <case-file-event-statements>
                <case-file-event-statement>
                  <code>ASCK</code>
                  <type>I</type>
                  <description-text>ASSIGNMENT OF OWNERSHIP NOT UPDATED AUTOMATICALLY</description-text>
                  <date>20140827</date>
                  <number>17</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>NA89</code>
                  <type>E</type>
                  <description-text>NOTICE OF ACCEPTANCE OF SEC. 8 &amp; 9 - E-MAILED</description-text>
                  <date>20131214</date>
                  <number>16</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>RNL7</code>
                  <type>Q</type>
                  <description-text>REGISTERED AND RENEWED (SEVENTH RENEWAL - 10 YRS)</description-text>
                  <date>20131214</date>
                  <number>15</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>89AG</code>
                  <type>O</type>
                  <description-text>REGISTERED - SEC. 8 (10-YR) ACCEPTED/SEC. 9 GRANTED</description-text>
                  <date>20131214</date>
                  <number>14</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>89AF</code>
                  <type>I</type>
                  <description-text>REGISTERED - COMBINED SECTION 8 (10-YR) &amp; SEC. 9 FILED</description-text>
                  <date>20131206</date>
                  <number>13</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>APRE</code>
                  <type>A</type>
                  <description-text>CASE ASSIGNED TO POST REGISTRATION PARALEGAL</description-text>
                  <date>20131214</date>
                  <number>12</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>E89R</code>
                  <type>I</type>
                  <description-text>TEAS SECTION 8 &amp; 9 RECEIVED</description-text>
                  <date>20131206</date>
                  <number>11</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>A7OK</code>
                  <type>O</type>
                  <description-text>AMENDMENT UNDER SECTION 7 - ISSUED</description-text>
                  <date>20131016</date>
                  <number>10</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>AMD7</code>
                  <type>I</type>
                  <description-text>SEC 7 REQUEST FILED</description-text>
                  <date>20131009</date>
                  <number>9</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>APRE</code>
                  <type>A</type>
                  <description-text>CASE ASSIGNED TO POST REGISTRATION PARALEGAL</description-text>
                  <date>20131011</date>
                  <number>8</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>ES7R</code>
                  <type>I</type>
                  <description-text>TEAS SECTION 7 REQUEST RECEIVED</description-text>
                  <date>20131009</date>
                  <number>7</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>TCCA</code>
                  <type>I</type>
                  <description-text>TEAS CHANGE OF CORRESPONDENCE RECEIVED</description-text>
                  <date>20061112</date>
                  <number>6</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>RNL6</code>
                  <type>Q</type>
                  <description-text>REGISTERED AND RENEWED (SIXTH RENEWAL - 10 YRS)</description-text>
                  <date>20040813</date>
                  <number>5</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>89AG</code>
                  <type>O</type>
                  <description-text>REGISTERED - SEC. 8 (10-YR) ACCEPTED/SEC. 9 GRANTED</description-text>
                  <date>20040813</date>
                  <number>4</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>89AF</code>
                  <type>I</type>
                  <description-text>REGISTERED - COMBINED SECTION 8 (10-YR) &amp; SEC. 9 FILED</description-text>
                  <date>20031210</date>
                  <number>3</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>RNL5</code>
                  <type>Q</type>
                  <description-text>REGISTERED AND RENEWED (FIFTH RENEWAL - 10 YRS)</description-text>
                  <date>19940901</date>
                  <number>2</number>
                </case-file-event-statement>
                <case-file-event-statement>
                  <code>REN4</code>
                  <type>R</type>
                  <description-text>REGISTERED AND RENEWED (FOURTH RENEWAL - 20 YRS)</description-text>
                  <date>19740527</date>
                  <number>1</number>
                </case-file-event-statement>
              </case-file-event-statements>
              <classifications>
                <classification>
                  <international-code-total-no>1</international-code-total-no>
                  <us-code-total-no>1</us-code-total-no>
                  <international-code>022</international-code>
                  <us-code>007</us-code>
                  <status-code>6</status-code>
                  <status-date>19830301</status-date>
                  <first-use-anywhere-date>18840101</first-use-anywhere-date>
                  <first-use-in-commerce-date>18840101</first-use-in-commerce-date>
                  <primary-code>007</primary-code>
                </classification>
              </classifications>
              <correspondent>
                <address-1>DENISE I. MROZ</address-1>
                <address-2>WOODCOCK WASHBURN LLP</address-2>
                <address-3>2929 ARCH ST</address-3>
                <address-4>CIRA CTR 12TH FL</address-4>
                <address-5>PHILADELPHIA PA 19104-2891</address-5>
              </correspondent>
              <case-file-owners>
                <case-file-owner>
                  <entry-number>1</entry-number>
                  <party-type>43</party-type>
                  <nationality>
                    <state>WA</state>
                  </nationality>
                  <legal-entity-type-code>03</legal-entity-type-code>
                  <party-name>SAMSON ROPE TECHNOLOGIES, INC.</party-name>
                  <address-1>2090 THORNTON ROAD</address-1>
                  <city>FERNDALE</city>
                  <state>WA</state>
                  <postcode>98248</postcode>
                </case-file-owner>
                <case-file-owner>
                  <entry-number>1</entry-number>
                  <party-type>42</party-type>
                  <nationality>
                    <state>DE</state>
                  </nationality>
                  <legal-entity-type-code>03</legal-entity-type-code>
                  <party-name>SAMSON OCEAN SYSTEMS, INC.</party-name>
                  <address-1>2090 THORNTON STREET</address-1>
                  <city>FERNDALE</city>
                  <state>WA</state>
                  <postcode>98248</postcode>
                  <name-change-explanation>NC42000001ASSIGNEE OF</name-change-explanation>
                </case-file-owner>
                <case-file-owner>
                  <entry-number>1</entry-number>
                  <party-type>30</party-type>
                  <nationality>
                    <country>US</country>
                  </nationality>
                  <legal-entity-type-code>01</legal-entity-type-code>
                  <party-name>TOLMAN, JAMPES P.</party-name>
                  <city>BOSTON</city>
                  <state>MA</state>
                </case-file-owner>
              </case-file-owners>
              <design-searches>
                <design-search>
                  <code>020119</code>
                </design-search>
                <design-search>
                  <code>020132</code>
                </design-search>
                <design-search>
                  <code>030101</code>
                </design-search>
                <design-search>
                  <code>040125</code>
                </design-search>
              </design-searches>
            </case-file>

# License
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, either version 3 of the
License, or (at your option) any later version.

   This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

   You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.