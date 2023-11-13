import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Iproduct } from '../Interfaces/iproduct';
import { IproductFilter } from '../Interfaces/iproductfilter';
import { Iproductadd } from '../Interfaces/product/iproductadd';

@Injectable({
  providedIn: 'root'
})
export class ProductlistService {

  BaseUrl: string = "https://localhost:7003/api/Products/All";
  ProductUrl: string = "https://localhost:7003/api/Products";

  constructor(private http: HttpClient) { }

  // -------------------- [ Get All Products ]
  getProducts(queryParams: IproductFilter): Observable<Iproduct[]> {

    // declaring an HttpParams() to add it to the url
    let params = new HttpParams();

    // looping over the object to to send only the non-empty param
    Object.keys(queryParams).forEach(key => {
      const value = queryParams[key];
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, value);
      }
    });

    return this.http.get<Iproduct[]>(this.BaseUrl, { params });

  }

  // -------------------- [ Get Product By Id ]
  GetProductById(id:number) : Observable<Iproduct>{
    return this.http.get<Iproduct>(this.ProductUrl + "/" +  id)
  }

  // -------------------- [ Delete Product ]
  DeleteProduct(id: number): Observable<void> {
    let params = new HttpParams();
    params = params.set('id', id.toString()); // Convert id to string if needed

    return this.http.delete<void>(this.ProductUrl, { params });
  }

  // -------------------- [ Add Product ]
  AddProduct(data : FormData): Observable<FormData> {

    return this.http.post<FormData>(this.ProductUrl, data);
  }




}


