import { Component, OnInit } from '@angular/core';
import { ACCESSMODIFIERS } from '../shared/consts';
import {HttpClient} from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { HttpResponse } from '@angular/common/http';
import {FormControl, Validators, PatternValidator} from '@angular/forms';



@Component({
  selector: 'app-form',
  templateUrl: './form.component.html',
  styleUrls: ['./form.component.css']
})
export class FormComponent implements OnInit {
  constructor(private http: HttpClient) { }
  expression: string="y(a,b,c,x)=ax^2+bx+c";
  methodName="exampleMethod";
  result="";
  isValid = false;
  
  accessModifiers=ACCESSMODIFIERS;
  selectedAccessModifier = 'public';
  selectedType = 'int';

   methodParameters: string = "";
  //get = this.http2.get('/api/values/Get/{expression}').subscribe();
  
  expressionControl = new FormControl('', [
    Validators.required,
    //Pattern
    Validators.pattern(/^[a-zA-Z0-9\w]+\([a-zA-Z0-9,]+\)\=[\w \* \/ \+ \- \^ \( \) \\ \{ \} \[ \] \. \,]+$/)
    //Validators.pattern('^[0-9]+')
  ]);

  methodNameControl = new FormControl('', [
    Validators.required
  ]);
  
  isValidForm()
  {
    this.expressionControl.valid && this.methodNameControl.valid ? this.isValid = true : this.isValid = false;
    
    console.log(this.expressionControl + '   ' + this.isValid);
      return this.isValid;
  }
  
  generateMethod()
  {
    this.result = '';
    const headers = new Headers();
    headers.set('Content-Type', 'application/json; charset=utf-8');
   //let body = JSON.parse(this.expression);
    this.http.post('http://localhost:5000/api/Values/GetMethodData/', {expression:this.expression}).subscribe(receivedData => {
    this.result = this.selectedAccessModifier + ' ' + 'double' + ' ' + this.methodName.toString() + '(' + 
      receivedData[0] + '){' + '\n' + receivedData[1] + '\n' + '}';
    });
  }

  
  ngOnInit() {
  }
}
