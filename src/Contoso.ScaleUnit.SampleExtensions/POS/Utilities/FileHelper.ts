export default class FileHelpers {

    public static convertBase64ToExcel = (base64data: string, fileName: string, elementName: string): void => {
        fileName += ` - ${new Date().toDateString()}`;
        let download: HTMLElement = document.getElementById(elementName);
        download.hidden = false;
        download.setAttribute("download", fileName);

        // Method 1 href attachment (using data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,)
        let href: string = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,' + base64data;
        download.setAttribute("href", href);

        // Method 2 (Blob creation and attachment to href)
        //let contentType: string = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;';
        // // Use any one of the blob creating methods.
        //let blob: Blob = FileHelpers.b64toBlob(base64data, contentType);
        //let blob: Blob = new Blob(base64data, { type: contentType });
        //let blob: Blob = new Blob([FileHelpers.s2ab(atob(base64data))], { type: contentType });
        //let blobUrl1: string = URL.createObjectURL(blob);
        //download.setAttribute("href", blobUrl1);
        

    }


    public static b64toBlob = (b64Data: string, contentType: string, sliceSize?: number): Blob => {
        contentType = contentType || '';
        sliceSize = sliceSize || 512;

        let byteCharacters: string = atob(b64Data);
        let byteArrays: Uint8Array[] = [];

        for (let offset: number = 0; offset < byteCharacters.length; offset += sliceSize) {
            let slice: string = byteCharacters.slice(offset, offset + sliceSize);

            let byteNumbers: any[] = new Array(slice.length);
            for (let i: number = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            let byteArray: Uint8Array = new Uint8Array(byteNumbers);

            byteArrays.push(byteArray);
        }

        let blob: Blob = new Blob(byteArrays, { type: contentType });
        return blob;
    }

    public static s2ab = (s: string): ArrayBuffer => {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (let i: number = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }
}